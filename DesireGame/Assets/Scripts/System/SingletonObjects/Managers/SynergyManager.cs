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
        private Dictionary<eSynergy, Action<SynergyParameter>> _OnSynergyChanges;

        #region ������
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            _synergyActivator = new();
            _OnSynergyChanges = new();
        }

        public void SubscribeToChanges(eSynergy synergy, Action<SynergyParameter> trigging) => _OnSynergyChanges[synergy] += trigging;
        
        public void UnsubscribeToChanges(eSynergy synergy, Action<SynergyParameter> trigging) => _OnSynergyChanges[synergy] -= trigging;

        public void RegisterCharSynergy(CharLightWeightInfo registrar, eSynergy synergy)
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
            if (_OnSynergyChanges[targetSynergy] == null)
                return;
            _OnSynergyChanges[targetSynergy].Invoke(new SynergyParameter()
            {
                triggingSynergy = targetSynergy,
                function = _synergyActivator[targetSynergy].GetSynergyByLevel().functionIndex
            });
        }

        public void DeleteCharSynergy(CharLightWeightInfo leaver, eSynergy synergy)
        {           
            if (synergy == eSynergy.None) return;
            if (_synergyActivator.ContainsKey(synergy))
            {
                _synergyActivator[synergy].SynergyMembers.Remove(leaver);

            }
            CheckSynergyChange(synergy);
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

    public struct CharLightWeightInfo
    {
        public long index;
        public long uid;

        public readonly CharBase SpecifyCharBase()
        {
            return CharManager.Instance.GetFieldChar(uid);
        }
    }
    

    public class SynergyContainer
    {
        public int DistinctMembers
        {
            get
            {
                var distinctList = SynergyMembers
                    .GroupBy(member => member.index)
                    .Select(g => g.First())
                    .ToList();
                return distinctList.Count;
            }
        }

        public eSynergy mySynergy;

        //public eSynergyLevel Level
        //{
        //    get
        //    {
        //        return GetSynergyByLevel().level;
        //    }
        //}
        //
        public List<CharLightWeightInfo> SynergyMembers;
        
        public SynergyContainer(eSynergy synergy)
        {
            SynergyMembers = new List<CharLightWeightInfo>();
            mySynergy = synergy;
        }

        public SynergyData GetSynergyByLevel()
        {
            var synergyInfo = DataManager.Instance.SynergyTriggerMap[mySynergy];
            var sortedKeys = new List<int>(synergyInfo.Keys);
            sortedKeys.Sort();

            int nowDistinct = DistinctMembers;

            // ������ 1�� �ó����� �ó��� �������� �ݵ�� �����ؾ� ��.
            int levelThreshold = 1;
            foreach (var threshold in sortedKeys)
            {               
                if(nowDistinct < threshold)
                {
                    return synergyInfo[levelThreshold];
                }
                levelThreshold = threshold;
            }
            return synergyInfo[levelThreshold];
        }

    }
}