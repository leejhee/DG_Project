using static Client.SystemEnum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections;

namespace Client
{
    public class SynergyContainer
    {
        private readonly eSynergy _mySynergy;
        private readonly eCharType _myCharType;

        private List<CharLightWeightInfo> _synergyMembers;
        public ReadOnlyCollection<CharLightWeightInfo> SynergyMembers => _synergyMembers.AsReadOnly();
        
        private HashSet<CharLightWeightInfo> _guestMemberSet;

        #region 생성자
        public SynergyContainer(eSynergy synergy, eCharType myCharType)
        {
            _synergyMembers = new();
            _guestMemberSet = new();
            _mySynergy = synergy;
            _myCharType = myCharType;
        }
        #endregion
        public override string ToString()
        {
            return $"{_mySynergy} 시너지, Members : {_synergyMembers.Count}, DistinctMembers : {DistinctMembers}";
        }
        
        #region Synergy Member Control

        public int MemberCount => _synergyMembers.Count;

        // 해당 시너지에 포함되는 멤버가 등록될 때 호출된다.
        public void Register(CharLightWeightInfo registrar)
        {
            if(_guestMemberSet.Contains(registrar))
                _guestMemberSet.Remove(registrar);
            if(!_synergyMembers.Contains(registrar))
                _synergyMembers.Add(registrar);

            if (CheckSynergyChange())
                SetCurrentSynergy();
            else
                GetCurrentSynergyBuff(registrar);

        }

        public void GuestRegister(CharLightWeightInfo guest)
        {
            _guestMemberSet.Add(guest);
            GetCurrentSynergyBuff(guest);
        }
        
        // 해당 시너지에 포함되는 멤버가 탈퇴할 때 호출된다.
        public void Delete(CharLightWeightInfo leaver)
        {
            if (_synergyMembers.Contains(leaver))
                _synergyMembers.Remove(leaver);

            if (_synergyMembers.Count == 0)
            {
                ClearSynergyBuff();
                return;           
            }
            
            KillLeaverBuff(leaver);

            if (CheckSynergyChange())
                SetCurrentSynergy();

        }

        public void GuestDelete(CharLightWeightInfo guestLeaver)
        {
            _guestMemberSet.Remove(guestLeaver);
            KillLeaverBuff(guestLeaver);
        }

        private void OnNewBuffDistributed(CharBase specifiedMember)
        {
            if (specifiedMember == false) return;
            specifiedMember.StartCoroutine(DelayedEvaluateCondition(
                new SynergyConditionInput()
                {
                    ChangedSynergy = _mySynergy, 
                    CharTypeContext = _myCharType,
                    RegistrarIndex = specifiedMember.Index
                },
                specifiedMember));
        }

        private void ScanAllMembers()
        {
            foreach(var member in _synergyMembers)
                OnNewBuffDistributed(member.SpecifyCharBase());
            foreach(var guest in _guestMemberSet)
                OnNewBuffDistributed(guest.SpecifyCharBase());
        }
        
        private IEnumerator DelayedEvaluateCondition(ConditionCheckInput param, CharBase target)
        {
            Debug.Log($"{target.GetID()}번 캐릭터에서 시너지 변경으로 인한 조건 검사");

            yield return null;
            
            target.FunctionInfo.EvaluateCondition(param);
        }
        #endregion

        #region Synergy Data Getter
        private List<SynergyData> _currentSynergyBuff = new();
        public int DistinctMembers
        {
            get
            {
                var distinctList = _synergyMembers
                    .GroupBy(member => member.Index)
                    .Select(g => g.First())
                    .ToList();
                return distinctList.Count;
            }
        }

        private List<SynergyData> GetSynergyByLevel()
        {
            var synergyInfo = DataManager.Instance.SynergyDataMap[_mySynergy];
            var sortedKeys = new List<int>(synergyInfo.Keys);
            sortedKeys.Sort();

            int nowDistinct = DistinctMembers;

            // 문턱이 1인 시너지는 시너지 종류별로 반드시 존재해야 함.
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
        private bool CheckSynergyChange()
        {
            var newSynergy = GetSynergyByLevel();
            if (_currentSynergyBuff == newSynergy)
            {
                return false;
            }
            
            _currentSynergyBuff = newSynergy;
            return true;
        }

        // 현재 적용되는 글로벌 시너지 버프를 얻는 함수
        private void GetCurrentSynergyBuff(CharLightWeightInfo receiver)
        {
            var caster = receiver.SpecifyCharBase();
            if (caster == false) return;

            foreach(var synergyData in _currentSynergyBuff)
            {
                if(synergyData.casterType == eSynergyRange.GLOBAL_ALLY)
                {
                    GetBuff(caster, synergyData);
                }
            }
            
            OnNewBuffDistributed(caster);
        }

        private void KillLeaverBuff(CharLightWeightInfo releaser)
        {
            var caster = releaser.SpecifyCharBase();
            if (caster == false) return;

            // 안전하게 하자.
            int count = synergyBuffRecords.Count;
            while (count-- > 0)
            {
                var record = synergyBuffRecords.Dequeue();
                if (record.Caster == caster)
                    continue;
                else
                    synergyBuffRecords.Enqueue(record);               
            }
        }

        // 시너지 버프 얻는 함수
        private void GetBuff(CharBase caster, SynergyData data)
        {
            #region GETTING PARAMETERS
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);
            if (funcData == null)
            {
                Debug.LogError("잘못 됐다잖냐 이녀석아 데이터 확인 다시해봐라.");
                return;
            }
            
            var intendedTargets = TargetStrategyFactory.CreateTargetStrategy
                (new TargettingStrategyParameter { Caster = caster, type = data.skillTarget })?.GetTargets();

            CharBase target = null;
            if (intendedTargets == null)
            {
                Debug.LogWarning($"현재 버프 {data.functionIndex}가 타겟이 null입니다. 시스템 제공 대상인지 확인하세요.");
            }
            else if (intendedTargets.Count == 0)
                Debug.Log($"현재 타겟이 아직 타게팅 타입 {data.skillTarget}에 따라 정해지지 않습니다.");
            else
                target = intendedTargets[0];
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

            caster.FunctionInfo.AddFunction(synergyFunction, data.buffTriggerTime);
            Debug.Log($"{caster.GetID()}번 캐릭터 {caster.name}에 synergy Function {funcData.Index}번 function 주입. " +
                                    $"기능 : {funcData.function}");
            //////////Add Record/////////
            synergyBuffRecords.Enqueue(new SynergyBuffRecord(caster, synergyFunction));
            #endregion

        }
        
        //TODO : 반드시 이 겹치는 생성부를 리팩토링 할 것.
        private void GetSynergySystemBuff(SynergyData data)
        {
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);
            var synergyFunction = FunctionFactory.FunctionGenerate(new BuffParameter()
            {
                CastChar = null,
                TargetChar = null,
                eFunctionType = funcData.function,
                FunctionIndex = funcData.Index
            });
            
            synergyBuffRecords.Enqueue(new SynergyBuffRecord(null, synergyFunction));
            synergyFunction.RunFunction(); // TODO : 단일로 그냥 돌아가게 하는 거긴 한데, 전역 클래스에서 관리하게 할지 고민할 것
        }

        private void ClearSynergyBuff()
        {
            while(synergyBuffRecords.Count > 0)
            {
                SynergyBuffRecord record = synergyBuffRecords.Dequeue();
                record?.KillSynergyBuff();
            }
        }
        
        // 시너지 갱신 함수
        private void SetCurrentSynergy()
        {
            ClearSynergyBuff();

            List<CharBase> casters = new();            
            foreach(var synergyData in _currentSynergyBuff)
            {
                // eSynergyRange에 따라서, Caster로 지정할 아군의 범위를 지정한다.
                switch (synergyData.casterType)
                {
                    case eSynergyRange.SYSTEM:
                        {
                            GetSynergySystemBuff(synergyData);
                            return;
                        }
                    case eSynergyRange.SELF:
                        foreach (var info in _synergyMembers)
                        {
                            casters.Add(info.SpecifyCharBase());
                        }
                        break;
                    case eSynergyRange.GLOBAL_ALLY:
                        casters = CharManager.Instance.GetAllySide(_myCharType);
                        break;
                    case eSynergyRange.GLOBAL_ENEMY:
                        casters = CharManager.Instance.GetEnemySide(_myCharType);
                        break;
                    default: break;
                }

                Debug.Log($"시너지 변경이 감지되었습니다. " +
                            $"Synergy {_mySynergy}에서 {DistinctMembers}명 달성하여 {synergyData.Index}번 버프 " +
                            $"casters {casters.Count}에게 들어감");

                // 캐스터 순회하여 각각 caster로 설정한 function을 만들어 add한다.
                foreach (var caster in casters)
                {
                    if (caster == false) continue;
                    GetBuff(caster, synergyData); 
                }
            }

            ScanAllMembers();
        }

        #endregion

    }

}