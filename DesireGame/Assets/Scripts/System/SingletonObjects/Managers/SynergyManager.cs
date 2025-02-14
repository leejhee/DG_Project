using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public void RegisterCharSynergy(CharBase charBase)
        {
            foreach(var synergy in charBase.CharSynergies)
            {
                if (!_synergyActivator.ContainsKey(synergy))
                    _synergyActivator.Add(synergy, new SynergyContainer());
                else
                    _synergyActivator[synergy].SynergyMembers.Add(charBase);
                OnRegisterSynergy?.Invoke(synergy);
            }
        }

        public void DeleteCharSynergy(CharBase charBase)
        {
            foreach (var synergy in charBase.CharSynergies)
            {
                _synergyActivator[synergy].SynergyMembers.Remove(charBase);
                OnDeleteSynergy?.Invoke(synergy);
            }
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
            set { DistinctMembers = value; }
        }

        public List<CharBase> SynergyMembers;
        
        public SynergyContainer()
        {
            DistinctMembers = 0;
            SynergyMembers = new List<CharBase>();
        }

    }
}