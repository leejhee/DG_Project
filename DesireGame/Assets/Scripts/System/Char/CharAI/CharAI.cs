using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;
using System;

namespace Client
{
    public class CharAI
    {
        public enum eAttackMode { None, Auto, Skill };
        #region Fields - target & states
        // 현재 이 AI 인스턴스를 가지고 있는 캐릭터
        private CharBase _charAgent;
        
        // 현재 공격 모드에 대해 지정된 전체 타겟들
        private List<CharBase> _cachedTargets = new();
        
        // 현재 캐릭터의 행동 결정 상태
        private PlayerState _currentState; // 새로운 모드 변경 여부
        
        // 현재 캐릭터의 공격 모드
        private eAttackMode _attackMode;
        
        // AI 내부 타이머의 리셋 필요 여부
        private bool _resetTimer = false;
        
        // 현재 스킬 타임라인 재생중 여부
        public bool IsSkillPlaying = false;
        
        // 타겟 지정 시 이벤트
        public Action<CharBase> OnTargetSet;    
        
        #endregion
        
        #region Fields - cc related

        private Coroutine _ccCoroutine = null;
        private bool _isCC = false;
        private eCCType _currentCCType;
        
        private bool _targetable = true;
        private bool _attackable = true;
        private bool _movable = true;
        private bool _skillable = true;

        #endregion
        public CharBase FinalTarget { get; private set; }// 우선 순위 계산의 최종 결과
        public bool isAIRun { get; set; } = false; // 이 캐릭터는 현재 AI가 작동중입니다.

        public CharAI(CharBase charAgent)
        {
            this._charAgent = charAgent;
        }

        /// <summary>
        /// 캐릭터 AI 행동 결정 타이머
        /// </summary>
        public IEnumerator UpdateAI(PlayerState initialState)
        {
            // 스킬 사용하는 동안에는 타이머 작동 중단
            // 현재 상태가 공격or이동에 따라 SetState() 호출 주기 바꿔줘야 함
            // 상태 변경 시 기존 타이머 중단

            _currentState = initialState;
            float actionInterval = GetActionInterval(_currentState);
            WaitForSeconds waitTime = new WaitForSeconds(actionInterval);

            while (true)
            {
                // 상태가 변경되었을 경우 타이머를 리셋
                if (_resetTimer)
                {
                    _resetTimer = false;
                    actionInterval = GetActionInterval(_currentState);
                    waitTime = new WaitForSeconds(actionInterval);
                }

                if (IsSkillPlaying)
                {
                    yield return waitTime;
                    continue;
                }
                
                _attackMode = SetAttackMode();
                SetStateByAttackMode(_attackMode);
                SetAction(_attackMode);

                yield return waitTime;
            }
        }
        

        /// <summary>
        /// 현재 상태에 따른 호출 간격 리턴
        /// </summary>
        /// <param name="newState"></param>
        private float GetActionInterval(PlayerState newState)
        {
            float interval = 0;

            switch (newState)
            {
                case PlayerState.ATTACK:
                    interval = 1 / _charAgent.CharStat.GetStat(eStats.NAS);
                    break;

                case PlayerState.MOVE:
                    interval = 1 / _charAgent.CharStat.GetStat(eStats.NMOVE_SPEED);
                    break;

                default:
                    interval = Time.deltaTime;
                    break;
            }

            return interval;
        }

        /// <summary>
        /// 마나 양에 따라 공격 모드 판단
        /// </summary>
        /// <returns></returns>
        private eAttackMode SetAttackMode()
        {
            if (!_skillable) return eAttackMode.Auto;
            // 스킬 사용 조건
            // 1: 최대 마나가 0보다 크다
            // 2: 현재 마나 >= 최대 마나
            bool condition1 = _charAgent.CharStat.GetStat(eStats.MAX_MANA) > 0;
            bool condition2 = _charAgent.CharStat.GetStat(eStats.N_MANA) >= _charAgent.CharStat.GetStat(eStats.MAX_MANA);

            if (condition1 && condition2)
                return eAttackMode.Skill;
            else
                return eAttackMode.Auto;
        }
        
        /// <summary>
        /// 상태 계산 및 행동 설정
        /// </summary>
        private void SetStateByAttackMode(eAttackMode attackMode)
        {
            switch (attackMode)
            {
                case eAttackMode.Auto:
                case eAttackMode.Skill:
                    ChangeState(PlayerState.ATTACK);
                    break;

                default:
                    ChangeState(PlayerState.IDLE);
                    break;
            }
        }

        private void ChangeState(PlayerState newState)
        {
            // 상태 변경 감지 시 
            if (_currentState != newState)
            {
                Debug.Log($"{_charAgent.CharData.charKorName}{_charAgent.GetID()} : {_currentState}에서 {newState}으로 상태 변경");
                _currentState = newState;
                _resetTimer = true; // 타이머 리셋 플래그 설정
            }
        }

        private void SetTarget(eSkillTargetType targetType)
        {
            if (!_targetable) return;
            var targettingGuide = TargetStrategyFactory.CreateTargetStrategy(new TargettingStrategyParameter()
            {
                type = targetType,
                Caster = _charAgent
            });
            _cachedTargets = targettingGuide.GetTargets();
            if (_cachedTargets == null)
            {
                Debug.LogWarning("No targets to Encounter");
                return;
            }
            FinalTarget = CharUtil.GetNearestInList(_charAgent, _cachedTargets); // 무조건 cached 중 최근접 대상
            if (!FinalTarget)
            {
                Debug.LogWarning("No target to Chase");
                return;
            }
            OnTargetSet?.Invoke(FinalTarget);
        }
        
        
        
        private void SetAction(eAttackMode attackMode)
        {
            SkillAIInfo info = _charAgent.CharSKillInfo.GetInfoByMode(attackMode);
            SetTarget(info.TargetType);
            if (!FinalTarget)
            {
                Debug.LogWarning("타겟 도중 섬멸. 무효화되어 다음 주기에 타겟 할당합니다.");
                return;
            }
            
            //데이터 기반 사거리 설정 및 행동 결정
            int skillRange = info.Range;
            Vector3 displacement = FinalTarget.CharTransform.position - _charAgent.CharTransform.position;
            _charAgent.CharTransform.localScale = displacement.x > 0 ? new Vector3(1, 1, 1) : new Vector3(1, 1, -1);
            var distance = displacement.magnitude;

            // 사거리와 비교 후 이동 결정
            bool inRange = distance <= skillRange + SystemConst.TOLERANCE || skillRange == 0;
            if (inRange)
            {
                if (!_attackable) return;
                _charAgent.CharAction.CharAttackAction(new CharAttackParameter(_cachedTargets, attackMode));
            }
            else
            {
                if (!_movable) return;
                _charAgent.Nav.stoppingDistance = skillRange;
                _charAgent.CharAction.CharMoveAction(new CharMoveParameter(FinalTarget));
            }
        }
        
        #region CC Control
        
        private IEnumerator StunBehavior(float duration)
        {
            _charAgent.Nav.isStopped = true;
            _charAgent.Move(false);
            
            if (duration > 0)
            {
                yield return new WaitForSeconds(duration);
                EndCCBehavior();
            }
        }

        // 매혹: 느린 속도로 시전자에게 접근
        private IEnumerator CharmBehavior(CharBase charmer, float duration)
        {
            float startTime = Time.time;
            float originalSpeed = _charAgent.CharStat.GetStat(eStats.NMOVE_SPEED);
            
            _charAgent.CharStat.AddStatModification(new StatModifier(
                eStats.MOVE_SPEED, eOpCode.Mul, eModifierRoot.CC, -0.5f));
            
            while (duration < 0 || Time.time - startTime < duration)
            {
                if (charmer && charmer.IsAlive)
                {
                    _charAgent.Nav.isStopped = false;
                    _charAgent.Nav.SetDestination(charmer.CharTransform.position);
                    _charAgent.Nav.speed = _charAgent.CharStat.GetStat(eStats.NMOVE_SPEED);
                    _charAgent.Move(true);
                }
                
                yield return new WaitForSeconds(0.1f); // 자주 업데이트
            }
            
            if (duration > 0)
                EndCCBehavior();
        }

        // 도발: 도발자만 공격, 평타만 사용
        private IEnumerator TauntBehavior(CharBase taunter, float duration)
        {
            float startTime = Time.time;
            
            // 공격속도 감소 적용
            _charAgent.CharStat.AddStatModification(new StatModifier(
                eStats.AS, eOpCode.Mul, eModifierRoot.CC, -0.5f));
            
            while (duration < 0 || Time.time - startTime < duration)
            {
                if (taunter && taunter.IsAlive)
                {
                    FinalTarget = taunter;
                    _cachedTargets.Clear();
                    _cachedTargets.Add(taunter);
                    
                    SkillAIInfo info = _charAgent.CharSKillInfo.GetInfoByMode(eAttackMode.Auto);
                    int skillRange = info.Range;
                    Vector3 displacement = taunter.CharTransform.position - _charAgent.CharTransform.position;
                    var distance = displacement.magnitude;
                    
                    bool inRange = distance <= skillRange + SystemConst.TOLERANCE || skillRange == 0;
                    if (inRange)
                    {
                        _charAgent.CharAction.CharAttackAction(new CharAttackParameter(_cachedTargets, eAttackMode.Auto));
                        yield return new WaitForSeconds(1f / _charAgent.CharStat.GetStat(eStats.NAS));
                    }
                    else
                    {
                        _charAgent.Nav.stoppingDistance = skillRange;
                        _charAgent.CharAction.CharMoveAction(new CharMoveParameter(taunter));
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    break;
                }
            }
            
            if (duration > 0)
                EndCCBehavior();
        }
        
        public IEnumerator StartCCBehavior(eCCType ccType, CharBase target, float duration = -1f)
        {
            if (_ccCoroutine != null)
            {
                _charAgent.StopCoroutine(_ccCoroutine);
            }
        
            _isCC = true;
            _currentCCType = ccType;
        
            switch (ccType)
            {
                case eCCType.STUN:
                    _ccCoroutine = _charAgent.StartCoroutine(StunBehavior(duration));
                    yield break;
                case eCCType.CHARM:
                    _ccCoroutine = _charAgent.StartCoroutine(CharmBehavior(target, duration));
                    yield break;
                case eCCType.TAUNT:
                    _ccCoroutine = _charAgent.StartCoroutine(TauntBehavior(target, duration));
                    yield break;
            }
        }
        
        public void EndCCBehavior()
        {
            if (_ccCoroutine != null)
            {
                _charAgent.StopCoroutine(_ccCoroutine);
                _ccCoroutine = null;
            }
        
            _isCC = false;
            _charAgent.AISwitch();
        }
        
        public void Charm(CharBase target)
        {
            TargetForcedFix(target);
            _attackable = false;
            _targetable = false;
        }

        public void Stun()
        {
            _attackable = false;
            _targetable = false;
            _movable = false;
        }
        
        public void Taunt(CharBase target)
        {
            TargetForcedFix(target);
            _targetable = false;
            AttackModeForcedFix(eAttackMode.Auto);
        }

        private void AttackModeForcedFix(eAttackMode mode)
        {
            if(mode == eAttackMode.Auto) _skillable = false;
            else if (mode == eAttackMode.Skill) _attackable = false;
        }
        
        private void TargetForcedFix(CharBase fixedTarget)
        {
            if (!fixedTarget) return;
            _cachedTargets.Clear();
            _cachedTargets.Add(fixedTarget);
        }

        public void RestoreState()
        {
            _attackable = true;
            _targetable = true;
            _movable = true;
            _skillable = true;
        }
        #endregion
        #region ONLY_FOR_TEST
        #if UNITY_EDITOR
        
        /// <summary>
        /// 런타임 상의 스킬 액션 확인
        /// 반드시 환경을 데이터를 참고하여 조성 후 테스트할 것.(타겟으로 할 대상들)
        /// </summary>
        public void TestSkillAction()
        {
            SetAction(eAttackMode.Skill);
        }

        public void TestAction(eAttackMode mode)
        {
            SkillAIInfo info = _charAgent.CharSKillInfo.GetInfoByMode(mode);
            SetTarget(info.TargetType);
            if (!FinalTarget)
            {
                Debug.LogWarning("타겟 도중 섬멸. 무효화되어 다음 주기에 타겟 할당합니다.");
                return;
            }
            
            //데이터 기반 사거리 설정 및 행동 결정
            int skillRange = info.Range;
            Vector3 displacement = FinalTarget.CharTransform.position - _charAgent.CharTransform.position;
            Debug.Log(displacement);
            _charAgent.CharTransform.localScale = displacement.z > 0 ? new Vector3(1, 1, 1) : new Vector3(1, 1, -1);
            var distance = displacement.magnitude;

            // 사거리와 비교 후 이동 결정
            bool inRange = distance <= SystemConst.GetWorldLength(skillRange) + SystemConst.TOLERANCE || skillRange == 0;
            if (inRange)
            {
                _charAgent.CharAction.CharAttackAction(new CharAttackParameter(_cachedTargets, mode));
            }
            else
            {
                Debug.LogError("스킬 사거리가 충분하지 않아, 스킬 사용이 불가합니다.\n" +
                               $"[스킬 사거리] {SystemConst.GetWorldLength(skillRange)} : , [현재 유닛 거리] : {distance}");
            }
        }
        
        public void TestSkillOnTarget(CharBase target, eAttackMode mode)
        {
            _charAgent.CharAction.CharAttackAction(new CharAttackParameter(new List<CharBase> {target}, mode));
        }
        #endif
        #endregion
    }
}