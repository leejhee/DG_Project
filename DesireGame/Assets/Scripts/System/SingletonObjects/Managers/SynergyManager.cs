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
        private Action<SynergyParameter> OnSynergyChanges;

        #region ������
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            _synergyActivator = new();
        }

        public void SubscribeToChanges(Action<SynergyParameter> trigging) => OnSynergyChanges += trigging;
        
        public void UnsubscribeToChanges(Action<SynergyParameter> trigging) => OnSynergyChanges -= trigging;

        public void RegisterCharSynergy(CharBase registrar, eSynergy synergy)
        {
            if (synergy == eSynergy.None) return;

            if (!_synergyActivator.ContainsKey(synergy))
            {
                _synergyActivator.Add(synergy, new SynergyContainer(synergy));
            }
            _synergyActivator[synergy].SynergyMembers.Add(registrar);

            CheckSynergyChange(synergy);
        }

        // ���� ���� �ɰ��°� ��Ÿ�� ����ޱ�.
        public void CheckSynergyChange(eSynergy targetSynergy)
        {
            OnSynergyChanges.Invoke(new SynergyParameter()
            {
                triggingSynergy = targetSynergy,
                functions = _synergyActivator[targetSynergy].GetSynergyByLevel().functionList
            });
        }

        public void DeleteCharSynergy(CharBase charBase)
        {
            foreach (var synergy in charBase.CharSynergies)
            {
                if (synergy == eSynergy.None) continue;

                _synergyActivator[synergy].SynergyMembers.Remove(charBase);              
                //OnDeleteSynergy?.Invoke(synergy); // Ʈ���� �ʿ��� ������ ����.
            }
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

    // �淮 ����. �� �ܼ��� ������ CharBase�� ã�� �� ������, CharManagere���� CharBase �����ϴ� �Լ��� �ʿ�.
    // �� �̰� �����ߴ���? SynergyManager�� CharBase���� �������� ���̷��� Trigger�� ������µ�,
    // CharBase�� ����ϸ� �ȵ��� ������ �ؼ�.
    // [TODO] : ���並 �ް� ���ô�.
    public struct CharLightWeightInfo
    {
        public long index;
        public long uid;
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

        public eSynergy mySynergy;

        public eSynergyLevel Level
        {
            get
            {
                return GetSynergyByLevel().level;
            }
        }

        public List<CharBase> SynergyMembers;
        
        public SynergyContainer(eSynergy synergy)
        {
            SynergyMembers = new List<CharBase>();
            mySynergy = synergy;
        }

        public SynergyData GetSynergyByLevel()
        {
            var synergyInfo = DataManager.Instance.SynergyTriggerMap[mySynergy];
            var sortedKeys = new List<int>(synergyInfo.Keys);
            sortedKeys.Sort();

            // ������ 1�� �ó����� �ó��� �������� �ݵ�� �����ؾ� ��.
            int levelThreshold = 1;
            foreach (var threshold in sortedKeys)
            {
                levelThreshold = threshold;
                if(levelThreshold < threshold)
                {
                    return synergyInfo[levelThreshold];
                }
            }
            return synergyInfo[levelThreshold];
        }

    }
}