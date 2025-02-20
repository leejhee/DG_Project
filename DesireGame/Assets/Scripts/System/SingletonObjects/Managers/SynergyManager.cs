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

        // 역할 따라 쪼개는거 스타일 리뷰받기.
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
                //OnDeleteSynergy?.Invoke(synergy); // 트리깅 쪽에서 구독한 사항.
            }
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
    // 왜 이걸 생각했느냐? SynergyManager의 CharBase에의 의존성을 줄이려고 Trigger을 만들었는데,
    // CharBase로 등록하면 안되지 않을까 해서.
    // [TODO] : 리뷰를 받고 씁시다.
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

            // 문턱이 1인 시너지는 시너지 종류별로 반드시 존재해야 함.
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