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
            if (OnSynergyChanges == null)
                return;
            OnSynergyChanges.Invoke(new SynergyParameter()
            {
                triggingSynergy = targetSynergy,
                functions = _synergyActivator[targetSynergy]?.GetSynergyByLevel()?.functionList
            });
        }

        public void DeleteCharSynergy(CharLightWeightInfo charBase, eSynergy synergy)
        {           
            if (synergy == eSynergy.None) return;
            if (_synergyActivator.ContainsKey(synergy))
            {
                _synergyActivator[synergy].SynergyMembers.Remove(charBase);

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

    // �淮 ����. �� �ܼ��� ������ CharBase�� ã�� �� ������, CharManagere���� CharBase �����ϴ� �Լ��� �ʿ�.
    // �� �̰� �����ߴ���? SynergyManager�� CharBase���� �������� ���̷��� Trigger�� ������µ�, CharBase�� ����ϸ� �ȵ��� ������ �ؼ�.
    public struct CharLightWeightInfo
    {
        public long index;
        public long uid;

        public readonly CharBase SpecifyCharBase()
        {
            return CharManager.Instance.GetFieldChar(uid);
        }
    }
    
    // ĳ���� - Ʈ���� - �Ŵ��� �̷������� ������ ���������µ�
    // �Ŵ������� ĳ���� ���̽� ��ü�� ������ ���� �� �Ǵ� �ƿ� ���ٸ�
    // ���� CharBase�� ������ �־�� �ұ�?
    // �׳� �� �淮 ������ ������ ������ ���? ��� ������ �ߴ�...

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

        public eSynergyLevel Level
        {
            get
            {
                return GetSynergyByLevel().level;
            }
        }

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