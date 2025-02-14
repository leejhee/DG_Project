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
        // eSynergy�� ������ registered�� �� 
        private readonly Action<SystemEnum.eSynergy> OnRegisterSynergy;
        private readonly Action<SystemEnum.eSynergy> OnDeleteSynergy;

        #region ������
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
                OnDeleteSynergy?.Invoke(synergy); // Ʈ���� �ʿ��� ������ ����.
            }
        }

        
        // ���� �׽�Ʈ�� �ϴ� �뵵
        public void ShowCurrentSynergies()
        {
            StringBuilder view = new("���� �ó���\n");
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