using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class SynergyManager : Singleton<SynergyManager>
    {
        private Dictionary<eSynergy, SynergyContainer> _synergyActivator;
        
        // 해당 구조는 그대로 유지.
        private readonly Action<eSynergy> OnRegisterSynergy;
        private readonly Action<eSynergy> OnDeleteSynergy;

        #region 생성자
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            _synergyActivator = new();
        }

        // 전제 : 덕지덕지 붙은 CharBase보다 걔가 가진 trigger가 가볍다.
        public void RegisterCharSynergy(CharBase registrar, eSynergy synergy)
        {
            if (synergy == eSynergy.None) return;

            if (!_synergyActivator.ContainsKey(synergy))
            {
                _synergyActivator.Add(synergy, new SynergyContainer());
            }
            _synergyActivator[synergy].SynergyMembers.Add(registrar);

            OnRegisterSynergy?.Invoke(synergy);         
        }

        public void DeleteCharSynergy(CharBase charBase)
        {
            foreach (var synergy in charBase.CharSynergies)
            {
                if (synergy == eSynergy.None) continue;

                _synergyActivator[synergy].SynergyMembers.Remove(charBase);              
                OnDeleteSynergy?.Invoke(synergy); // 트리깅 쪽에서 구독한 사항.
            }
        }

        public void CheckTargetSynergyLevel(eSynergy targetSynergy)
        {

        }


        public void DistributeSynergyBuff(eSynergy targetSynergy)
        {

        }


        #region Test_Method
        // 씬상 테스트만 하는 용도
        public void ShowCurrentSynergies()
        {
            StringBuilder view = new("현재 시너지\n");
            foreach(var kvp in _synergyActivator)
            {
                view.AppendLine($"eSynergy : {kvp.Key}, " +
                                $"distinctmembers : {kvp.Value.DistinctMembers}, " +
                                $"members : {kvp.Value.SynergyMembers.Count}");
            }
            Debug.Log(view.ToString());
        }
        #endregion

    }





    public class SynergyContainer
    {
        public int DistinctMembers
        {
            get
            {
                var distinctList = SynergyMembers.Distinct(new CharComparer()).ToList();
                return distinctList.Count;
            }
        }

        public eSynergyLevel Level;

        public List<CharBase> SynergyMembers;
        
        public SynergyContainer()
        {
            SynergyMembers = new List<CharBase>();
            Level = eSynergyLevel.None;
        }

    }
}