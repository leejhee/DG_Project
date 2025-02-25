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
        private Action<SynergyParameter> OnSynergyChanges;

        #region 생성자
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

        // 역할 따라 쪼개는거 스타일 리뷰받기.
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

    // 경량 구조. 이 단서로 무조건 CharBase를 찾을 수 있으나, CharManagere에서 CharBase 리턴하는 함수가 필요.
    // 왜 이걸 생각했느냐? SynergyManager의 CharBase에의 의존성을 줄이려고 Trigger을 만들었는데, CharBase로 등록하면 안되지 않을까 해서.
    public struct CharLightWeightInfo
    {
        public long index;
        public long uid;

        public readonly CharBase SpecifyCharBase()
        {
            return CharManager.Instance.GetFieldChar(uid);
        }
    }
    
    // 캐릭터 - 트리거 - 매니저 이런식으로 연결이 느슨해졌는데
    // 매니저에서 캐릭터 베이스 자체에 접근할 일이 잘 또는 아예 없다면
    // 굳이 CharBase를 가지고 있어야 할까?
    // 그냥 저 경량 정보로 가지고 있으면 어떨까? 라는 생각을 했다...

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

            // 문턱이 1인 시너지는 시너지 종류별로 반드시 존재해야 함.
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