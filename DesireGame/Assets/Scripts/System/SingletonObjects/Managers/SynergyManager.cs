using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// 시너지 관리 모듈
    /// </summary>
    public class SynergyManager : Singleton<SynergyManager>
    {
        private readonly Dictionary<eCharType, Dictionary<eSynergy, SynergyContainer>> _synergyMembers = new();
        private readonly Dictionary<eCharType, Dictionary<eSynergy, SynergyAnchor>> _anchors = new();

        private SynergyRouter _router;
        private bool _rebuilding;
        
        #region 생성자
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            _router = new SynergyRouter(this);          //라우터(분배기) 초기화
            GameManager.Instance.AddOnUpdate(Update);   //시너지 관련 업데이트 필요
        }

        public void Reset()
        {
            _synergyMembers.Clear();
            _anchors.Clear();
        }
        
        public ReadOnlyCollection<CharLightWeightInfo> GetInfo(eCharType side, eSynergy synergy)
        {
            if(_synergyMembers.ContainsKey(side))
                if(_synergyMembers[side].ContainsKey(synergy))
                    return _synergyMembers[side][synergy].SynergyMembers;
            return null;
        }
        
        /// <summary> 캐릭터 단위 시너지 등록 </summary>
        /// <param name="registrar"> 가입자 정보 </param>
        public void RegisterCharSynergy(CharLightWeightInfo registrar)
        {
            var side = registrar.Side;

            if (!_synergyMembers.ContainsKey(side)) 
                _synergyMembers.Add(side, new Dictionary<eSynergy, SynergyContainer>());
            
            foreach (var synergy in registrar.SynergyList)
            {
                RegisterSynergy(registrar, synergy);
            }
            
            var others = _synergyMembers[side].Keys.Except(registrar.SynergyList).ToList();
            foreach (var other in others)
            {
                _synergyMembers[side][other].GuestRegister(registrar, _rebuilding);
            }
            
        }
        
        public void DeleteCharSynergy(CharLightWeightInfo leaver)
        {
            var side = leaver.Side;

            foreach (eSynergy synergy in leaver.SynergyList)
            {
                DeleteSynergy(leaver, synergy);
            }
            
            var others = _synergyMembers[side].Keys.Except(leaver.SynergyList).ToList();
            foreach (var other in others)
            {
                _synergyMembers[side][other].GuestDelete(leaver);
            }

        }


        /// <summary> 시너지 등록 단위 </summary>
        /// <param name="registrar"> 가입자 정보 </param>
        /// <param name="synergy"> 가입할 시너지 </param>
        public void RegisterSynergy(CharLightWeightInfo registrar, eSynergy synergy)
        {
            if (synergy == eSynergy.None) return;
            var side = registrar.Side;
            
            
            if (!_synergyMembers.ContainsKey(side))
                _synergyMembers.Add(side, new Dictionary<eSynergy, SynergyContainer>());
            
            var synergyActivator = _synergyMembers[side];
            if (!synergyActivator.TryGetValue(synergy, out var ct))
            {
                ct = new SynergyContainer(synergy, side);
                synergyActivator.TryAdd(synergy, ct);
                _router.Wire(ct);

                var fams = CharManager.Instance.GetOneSide(side);
                if (fams != null)
                {
                    foreach (var cb in fams)
                    {
                        var info = cb.GetCharSynergyInfo();
                        if(!info.SynergyList.Contains(synergy))
                            ct.GuestRegister(info, _rebuilding);
                    }
                }
            }
            
            //if (!synergyActivator.ContainsKey(synergy))
            //{
            //    _synergyMembers[side].Add(synergy, new SynergyContainer(synergy, side));
            //    var allChars = CharManager.Instance.GetOneSide(side);
            //    foreach (var charBase in allChars)
            //    {
            //        CharLightWeightInfo info = charBase.GetCharSynergyInfo(); // 있으면 생성자, 없으면 직접 구성
            //        if (!info.SynergyList.Contains(synergy))
            //            synergyActivator[synergy].GuestRegister(info);
            //    }
            //}
            //_synergyMembers[side][synergy].Register(registrar);
            ct.Register(registrar);
        }

        public void DeleteSynergy(CharLightWeightInfo leaver, eSynergy synergy)
        {           
            if (synergy == eSynergy.None) return;
            var side = leaver.Side;
            
            if (_synergyMembers.TryGetValue(side, out var bySynergy) && bySynergy.TryGetValue(synergy, out var ct))
            {
                ct.Delete(leaver);
                if (ct.MemberCount == 0) bySynergy.Remove(synergy);
            }
        }
        
        public SynergyAnchor GetOrCreateAnchor(eCharType side, eSynergy syn)
        {
            if (!_anchors.TryGetValue(side, out var bySyn))
                bySyn = _anchors[side] = new Dictionary<eSynergy, SynergyAnchor>();

            if (!bySyn.TryGetValue(syn, out var a))
                a = bySyn[syn] = new SynergyAnchor(side, syn);

            return a;
        }
        
        private void Update()
        {
            foreach (Dictionary<eSynergy, SynergyAnchor> bySyn in _anchors.Values)
                foreach (SynergyAnchor a in bySyn.Values)
                    a.Tick();
        }
        
        /// <summary>
        /// 스테이지 초기화시 캐릭터들에 시너지 분배
        /// </summary>
        public void RebuildFromFieldAndDistribute()
        {
            _rebuilding = true;
            _synergyMembers.Clear();
            _anchors.Clear();

            foreach (var side in new[] { eCharType.ALLY, eCharType.ENEMY })
            {
                var list = CharManager.Instance.GetOneSide(side);
                if (list == null) continue;

                foreach (var cb in list)
                {
                    var info = cb.GetCharSynergyInfo();
                    RegisterCharSynergy(info);
                }
            }

            _rebuilding = false;

            foreach (var bySyn in _synergyMembers.Values)
                foreach (var cont in bySyn.Values)
                    _router.ApplyAll(cont);
        }
        
        #region Test_Method
        // 씬상 테스트만 하는 용도
        public void ShowCurrentSynergies()
        {
            StringBuilder view = new("현재 시너지\n");
            foreach (var value in _synergyMembers)
            {
                view.AppendLine($"{value.Key} 사이드 시너지 :");
                foreach (var syn in value.Value.Values)
                {
                    view.AppendLine(syn.ToString());
                }
            }
            Debug.Log(view.ToString());
        }
        #endregion

    }

}