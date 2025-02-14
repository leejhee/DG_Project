using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Client
{
    public class SynergyManager : Singleton<SynergyManager>
    {
        private Dictionary<SystemEnum.eSynergy, SynergyContainer> _synergyActivator;
        // eSynergy의 유닛이 registered될 때 
        private readonly Action<SystemEnum.eSynergy> OnRegisterSynergy;
        private readonly Action<SystemEnum.eSynergy> OnDeleteSynergy;

        #region 생성자
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            _synergyActivator = new();
        }

        public void RegisterCharSynergy(CharBase charBase)
        {
            foreach(var synergy in charBase.CharSynergies)
            {
                if (synergy == SystemEnum.eSynergy.None) continue;

                if (!_synergyActivator.ContainsKey(synergy))
                {
                    _synergyActivator.Add(synergy, new SynergyContainer());
                }

                _synergyActivator[synergy].SynergyMembers.Add(charBase);
                OnRegisterSynergy?.Invoke(synergy);
            }
        }

        public void DeleteCharSynergy(CharBase charBase)
        {
            foreach (var synergy in charBase.CharSynergies)
            {
                if (synergy == SystemEnum.eSynergy.None) continue;

                _synergyActivator[synergy].SynergyMembers.Remove(charBase);              
                OnDeleteSynergy?.Invoke(synergy); // 트리깅 쪽에서 구독한 사항.
            }
        }

        
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

        public List<CharBase> SynergyMembers;
        
        public SynergyContainer()
        {
            SynergyMembers = new List<CharBase>();
        }

    }
}