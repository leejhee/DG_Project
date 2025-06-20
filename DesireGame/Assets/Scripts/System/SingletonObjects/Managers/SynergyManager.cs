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
        // new! 타입별로 시너지가 나와야 한다.
        private Dictionary<eCharType, Dictionary<eSynergy, SynergyContainer>> _synergyMembers;
        
        private Dictionary<eSynergy, SynergyContainer> _synergyActivator;
        //public ReadOnlyCollection<CharLightWeightInfo> GetInfo(eSynergy synergy)
        //{
        //    return _synergyActivator.ContainsKey(synergy) ? _synergyActivator[synergy].SynergyMembers : null;
        //}

        public ReadOnlyCollection<CharLightWeightInfo> GetInfo(eCharType side, eSynergy synergy)
        {
            if(_synergyMembers.ContainsKey(side))
                if(_synergyMembers[side].ContainsKey(synergy))
                    return _synergyMembers[side][synergy].SynergyMembers;
            return null;
        }
        
        #region 생성자
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            //_synergyActivator = new();
            
            _synergyMembers = new();
        }

        public void Reset()
        {
            //_synergyActivator.Clear();
            _synergyMembers.Clear();
        }
        
        /// <summary> 캐릭터 단위 시너지 등록 </summary>
        /// <param name="registrar"> 가입자 정보 </param>
        public void RegisterCharSynergy(CharLightWeightInfo registrar)
        {
            var side = registrar.Side;
            var mySynergies = registrar.SynergyList;
            
            if(!_synergyMembers.ContainsKey(side))
                _synergyMembers.Add(side, new Dictionary<eSynergy, SynergyContainer>());
            //var otherSynergies = _synergyActivator.Keys.Except(mySynergies).ToList();
            /////////////////////////////
            foreach (var synergy in registrar.SynergyList)
            {
                RegisterSynergy(registrar, synergy);
            }

            //foreach(var other in otherSynergies)
            //{
            //    _synergyActivator[other].GuestRegister(registrar);
            //}
            /////////////////////////////
            
            var others = _synergyMembers[side].Keys.Except(mySynergies).ToList();
            foreach (var other in others)
            {
                _synergyMembers[side][other].GuestRegister(registrar);
            }
            
        }
        
        public void DeleteCharSynergy(CharLightWeightInfo leaver)
        {
            var side = leaver.Side;
            var mySynergies = leaver.SynergyList;
            //var otherSynergies = _synergyActivator.Keys.Except(mySynergies).ToList();

            foreach (var synergy in leaver.SynergyList)
            {
                DeleteSynergy(leaver, synergy);
            }

            //foreach (var other in otherSynergies)
            //{
            //    _synergyActivator[other].GuestDelete(leaver);
            //}
            ////////////////////////////
            
            var others = _synergyMembers[side].Keys.Except(mySynergies).ToList();
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
            var side = registrar.Side;
            if (synergy == eSynergy.None) return;
            
            /////////////////////////////////////
            //
            //if (!_synergyActivator.ContainsKey(synergy))
            //{
            //    _synergyActivator.Add(synergy, new SynergyContainer(synergy, registrar.Side));
            //}
            //_synergyActivator[synergy].Register(registrar);
            //
            /////////////////////////////////////
            var synergyActivator = _synergyMembers[side];
            if (!synergyActivator.ContainsKey(synergy))
            {
                _synergyMembers[side].Add(synergy, new SynergyContainer(synergy, side));
            }
            _synergyMembers[side][synergy].Register(registrar);
            
        }

        public void DeleteSynergy(CharLightWeightInfo leaver, eSynergy synergy)
        {           
            var side = leaver.Side;
            if (synergy == eSynergy.None) return;
            
            /////////////
            //if (_synergyActivator.ContainsKey(synergy))
            //{
            //    _synergyActivator[synergy].Delete(leaver);
            //}
            //
            //if (_synergyActivator[synergy].MemberCount == 0)
            //{
            //    _synergyActivator.Remove(synergy);
            //}
            /////////////

            if (_synergyMembers[side].ContainsKey(synergy))
            {
                _synergyMembers[side][synergy].Delete(leaver);
            }

            if (_synergyMembers[side][synergy].MemberCount == 0)
            {
                _synergyMembers[side].Remove(synergy);
            }
        }

        #region Test_Method
        // 씬상 테스트만 하는 용도
        public void ShowCurrentSynergies()
        {
            StringBuilder view = new("현재 시너지\n");
            //foreach(var value in _synergyActivator.Values)
            //{
            //    view.AppendLine(value.ToString());
            //}
            //Debug.Log(view.ToString());

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