using static Client.SystemEnum;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Collections.ObjectModel;

namespace Client
{
    public class SynergyContainer
    {
        private readonly eSynergy mySynergy;

        private List<CharLightWeightInfo> _synergyMembers;
        public ReadOnlyCollection<CharLightWeightInfo> SynergyMembers => _synergyMembers.AsReadOnly();

        #region 생성자
        public SynergyContainer(eSynergy synergy)
        {
            _synergyMembers = new();
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
                GetCurrentSynergyBuff(registrar);
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

        private Queue<SynergyBuffRecord> synergyBuffRecords = new();
        
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

        // 현재 적용되는 글로벌 시너지 버프를 얻는 함수
        public void GetCurrentSynergyBuff(CharLightWeightInfo receiver)
        {
            var caster = receiver.SpecifyCharBase();
            if (caster == null) return;

            foreach(var synergyData in _currentSynergyBuff)
            {
                if(synergyData.casterType == eSynergyRange.GLOBAL_ALLY)
                {
                    GetBuff(caster, synergyData);
                }
            }

        }

        // 시너지 버프 얻는 함수
        public void GetBuff(CharBase caster, SynergyData data)
        {
            #region GETTING PARAMETERS
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);
            if (funcData == null)
            {
                Debug.LogError("잘못 됐다잖냐 이녀석아 데이터 확인 다시해봐라.");
                return;
            }
            
            var IntendedTargets = TargetStrategyFactory.CreateTargetStrategy
                (new TargettingStrategyParameter() { Caster = caster, type = data.skillTarget }).GetTargets();

            CharBase target = null;
            if (IntendedTargets.Count == 0)
                Debug.Log($"현재 타겟이 타게팅 타입 {data.skillTarget}에 따라 정해지지 않습니다.");
            else
                target = IntendedTargets[0];
            #endregion

            #region ADD BUFF AND RECORD
            //////////Add Buff///////////
            var synergyFunction = FunctionFactory.FunctionGenerate(new BuffParameter()
            {
                CastChar = caster,
                TargetChar = target,
                eFunctionType = funcData.function,
                FunctionIndex = funcData.Index
            });

            //TODO : FunctionInfo에 '전투 시작'에 대한 지시 철회할 것. 시너지컨테이너에서 책임 진다.
            //TODO : SynergyContainer에서 Distributer로 객체 분리할지 생각해볼 것
            caster.FunctionInfo.AddFunction(synergyFunction, data.buffTriggerTime);
            Debug.Log($"{caster.GetID()}번 캐릭터 {caster.name}에 synergy Function {funcData.Index}번 function 주입. " +
                                    $"기능 : {funcData.function}");
            //////////Add Record/////////
            synergyBuffRecords.Enqueue(new SynergyBuffRecord(caster, synergyFunction));
            #endregion
        }

        // 시너지 갱신 함수
        public void SetCurrentSynergy()
        {
            // Reset Record
            while(synergyBuffRecords.Count > 0)
            {
                var record = synergyBuffRecords.Dequeue();
                if (record.Caster) // 만약 팔아서 나가고 Destroy되면 null일 거라서
                    record.KillSynergyBuff();
            }

            List<CharBase> casters = new();            
            foreach(var synergyData in _currentSynergyBuff)
            {
                // eSynergyRange에 따라서, Caster로 지정할 아군의 범위를 지정한다.
                switch (synergyData.casterType)
                {
                    case eSynergyRange.ONCE:                        
                        break;
                    case eSynergyRange.SELF:
                        foreach (var info in _synergyMembers)
                            casters.Add(info.SpecifyCharBase());
                        break;
                    case eSynergyRange.GLOBAL_ALLY:
                        casters = CharManager.Instance.GetOneSide(eCharType.ALLY);
                        break;
                    case eSynergyRange.GLOBAL_ENEMY:
                        casters = CharManager.Instance.GetOneSide(eCharType.ENEMY);
                        break;
                    default: break;
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
                    GetBuff(caster, synergyData);                       
                }
            }            
        }

        #endregion

    }

    public class SynergyBuffRecord
    {
        public CharBase Caster { get; private set; }
        public FunctionBase BuffFunction { get; private set; }

        public SynergyBuffRecord(CharBase caster, FunctionBase buffFunction)
        {
            Caster = caster;
            BuffFunction = buffFunction;
        }

        public void KillSynergyBuff()
        {
            Caster.FunctionInfo.KillFunction(BuffFunction);
            Debug.Log($"{Caster.name}의 버프 삭제 : {BuffFunction.functionType}");
        }

    }


}