using static Client.SystemEnum;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Client
{
    public class SynergyContainer
    {
        private eSynergy mySynergy;

        private List<CharLightWeightInfo> _synergyMembers;

        private List<CharLightWeightInfo> _buffReceiverSignature;

        #region 생성자
        public SynergyContainer(eSynergy synergy)
        {
            _synergyMembers = new();
            _buffReceiverSignature = new();
            mySynergy = synergy;
        }

        public override string ToString()
        {
            return $"{mySynergy} 시너지, Members : {_synergyMembers.Count}, DistinctMembers : {DistinctMembers}";
        }

        #endregion

        #region Synergy Member Control

        public int MemberCount => _synergyMembers.Count;

        // 해당 시너지에 포함되는 멤버가 등록될 때 호출된다.
        public void Register(CharLightWeightInfo registrar)
        {
            if(!_synergyMembers.Contains(registrar))
                _synergyMembers.Add(registrar);

            if (CheckSynergyChange())
                SetCurrentSynergy();
            else
                GetCurrentSynergyBuff();
        }

        // 해당 시너지에 포함되는 멤버가 탈퇴할 때 호출된다.
        public void Delete(CharLightWeightInfo leaver)
        {
            if (_synergyMembers.Contains(leaver))
                _synergyMembers.Remove(leaver);

            if (CheckSynergyChange())
                SetCurrentSynergy();
        }

        #endregion

        #region Synergy Data Getter
        private List<SynergyData> _currentSynergyBuff = new();
        public int DistinctMembers
        {
            get
            {
                var distinctList = _synergyMembers
                    .GroupBy(member => member.index)
                    .Select(g => g.First())
                    .ToList();
                return distinctList.Count;
            }
        }

        public List<SynergyData> GetSynergyByLevel()
        {
            var synergyInfo = DataManager.Instance.SynergyDataMap[mySynergy];
            var sortedKeys = new List<int>(synergyInfo.Keys);
            sortedKeys.Sort();

            int nowDistinct = DistinctMembers;

            // 문턱이 1인 시너지는 시너지 종류별로 반드시 존재해야 함.
            // 하지만 지금은 1짜리는 안만들어놨다. 그니까 활성화만 생각하자.
            int levelThreshold = 1;
            foreach (var threshold in sortedKeys)
            {
                if (nowDistinct < threshold)
                {
                    break;
                }
                levelThreshold = threshold;
            }

            if (!synergyInfo.ContainsKey(levelThreshold)) 
                return new List<SynergyData>();
            return synergyInfo[levelThreshold];
        }
        #endregion

        #region Synergy Buff Managing


        public Action OnSynergyChanges = null;
        
        // 시너지가 갱신되어야 하는지에 대한 체크 함수
        public bool CheckSynergyChange()
        {
            var newSynergy = GetSynergyByLevel();
            if (_currentSynergyBuff == newSynergy)
                return false;
            else
            {
                _currentSynergyBuff = newSynergy;
                return true;
            }
        }

        // 시너지 버프 얻는 함수
        public void GetCurrentSynergyBuff()
        {

        }

        // 시너지 갱신 함수
        public void SetCurrentSynergy()
        {
            var newSynergyData = GetSynergyByLevel();
            if(newSynergyData != null)
            {                
                OnSynergyChanges?.Invoke();
                OnSynergyChanges = null;
                _currentSynergyBuff = newSynergyData;

                List<CharBase> casters = new();
                
                foreach(var synergyData in _currentSynergyBuff)
                {
                    // eSynergyRange에 따라서, Caster로 지정할 아군의 범위를 지정한다.
                    if (synergyData.casterType == eSynergyRange.SELF)
                    {
                        foreach (var info in _synergyMembers)
                            casters.Add(info.SpecifyCharBase());                       
                    }
                    else if (synergyData.casterType == eSynergyRange.GLOBAL_ALLY)
                    {
                        casters = CharManager.Instance.GetOneSide(eCharType.ALLY);
                    }
                    else if(synergyData.casterType == eSynergyRange.GLOBAL_ENEMY)
                    {
                        casters = CharManager.Instance.GetOneSide(eCharType.ENEMY);
                    }

                    Debug.Log($"시너지 변경이 감지되었습니다. " +
                    $"Synergy {mySynergy}에서 {DistinctMembers}명 달성하여 {synergyData.Index}번 버프 " +
                    $"casters {casters.Count}에게 들어감");

                    foreach(var caster in casters)
                    {
                        Debug.Log(caster.name);
                    }

                    // 캐스터 순회하여 각각 caster로 설정한 function을 만들어 add한다.
                    foreach (var caster in casters)
                    {
                        var funcData = DataManager.Instance.GetData<FunctionData>(synergyData.functionIndex);
                        if (funcData == null)
                        {
                            Debug.LogError("잘못 됐다잖냐 이녀석아 데이터 확인 다시해봐라.");
                            continue;
                        }

                        if (FunctionFactory.FunctionGenerate(new BuffParameter()
                        {
                            CastChar = caster,
                            TargetChar = null,
                            FunctionIndex = funcData.Index,
                            eFunctionType = funcData.function
                        }) is not SynergyFunction synergyFunc)
                        {
                            Debug.LogError("너 시너지 말고 다른 펑션 가져오려 했지 딱걸렸어!");
                            continue;
                        }

                        // eSkillTargetType에 따라서, Target으로 지정할 유닛의 범위를 지정한다.
                        // 시너지 변경 경우 새로 할당된 function에 대체되기 위한 구독도 한다.
                        synergyFunc.DelayedInit(synergyData.skillTarget, OnSynergyChanges);
                        caster.FunctionInfo.AddFunction(synergyFunc);
                        Debug.Log($"{caster.GetID()}번 캐릭터 {caster.name}에 synergy Function {funcData.Index}번 function 주입. " +
                                    $"기능 : {funcData.function}");
                    }
                    


                }
            }
        }

        #endregion

    }
}