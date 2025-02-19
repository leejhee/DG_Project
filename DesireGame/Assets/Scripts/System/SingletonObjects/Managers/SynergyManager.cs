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
        
        // �ش� ������ �״�� ����.
        private readonly Action<eSynergy> OnRegisterSynergy;
        private readonly Action<eSynergy> OnDeleteSynergy;

        #region ������
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            _synergyActivator = new();
        }

        // ���� : �������� ���� CharBase���� �°� ���� trigger�� ������.
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
                OnDeleteSynergy?.Invoke(synergy); // Ʈ���� �ʿ��� ������ ����.
            }
        }

        public void CheckTargetSynergyLevel(eSynergy targetSynergy)
        {

        }


        public void DistributeSynergyBuff(eSynergy targetSynergy)
        {

        }


        #region Test_Method
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