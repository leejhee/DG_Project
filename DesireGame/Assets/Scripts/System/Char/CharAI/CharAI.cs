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
        private CharBase charAgent;
        
        // 현재 공격 모드에 대해 지정된 전체 타겟들
        private List<CharBase> cachedTargets;
        
        // 현재 캐릭터의 행동 결정 상태
        private PlayerState currentState; // 새로운 모드 변경 여부
        
        // 현재 캐릭터의 공격 모드
        private eAttackMode _attackMode;
        
        // AI 내부 타이머의 리셋 필요 여부
        private bool resetTimer = false;
        
        // 현재 스킬 타임라인 재생중 여부
        public bool isSkillPlaying = false;
        
        // 타겟 지정 시 이벤트
        public Action<CharBase> OnTargetSet;    
        
        #endregion
        
        #region Fields - cc related

        public bool Targetable = true;
        public bool Attackable = true;
        public bool Movable = true;

        #endregion
        public CharBase FinalTarget { get; private set; }// 우선 순위 계산의 최종 결과
        public bool isAIRun { get; set; } = false; // 이 캐릭터는 현재 AI가 작동중입니다.

        public CharAI(CharBase charAgent)
        {
            this.charAgent = charAgent;
        }

        /// <summary>
        /// 캐릭터 AI 행동 결정 타이머
        /// </summary>
        /// <param name="newState"></param>
        public IEnumerator UpdateAI(PlayerState initialState)
        {
            // 스킬 사용하는 동안에는 타이머 작동 중단
            // 현재 상태가 공격or이동에 따라 SetState() 호출 주기 바꿔줘야 함
            // 상태 변경 시 기존 타이머 중단

            currentState = initialState;
            float actionInterval = GetActionInterval(currentState);
            WaitForSeconds waitTime = new WaitForSeconds(actionInterval);

            while (true)
            {
                // 상태가 변경되었을 경우 타이머를 리셋
                if (resetTimer)
                {
                    resetTimer = false;
                    actionInterval = GetActionInterval(currentState);
                    waitTime = new WaitForSeconds(actionInterval);
                }

                if (isSkillPlaying)
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
        /// <returns></returns>
        private float GetActionInterval(PlayerState newState)
        {
            float interval = 0;

            switch (newState)
            {
                case PlayerState.ATTACK:
                    interval = 1 / charAgent.CharStat.GetStat(eStats.NAS);
                    break;

                case PlayerState.MOVE:
                    interval = 1 / charAgent.CharStat.GetStat(eStats.NMOVE_SPEED);
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
            // 스킬 사용 조건
            // 1: 최대 마나가 0보다 크다
            // 2: 현재 마나 >= 최대 마나
            bool condition1 = charAgent.CharStat.GetStat(eStats.MAX_MANA) > 0;
            bool condition2 = charAgent.CharStat.GetStat(eStats.N_MANA) >= charAgent.CharStat.GetStat(eStats.MAX_MANA);

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
            if (currentState != newState)
            {
                Debug.Log($"{charAgent.CharData.charName}{charAgent.GetID()} : {currentState}에서 {newState}으로 상태 변경");
                currentState = newState;
                resetTimer = true; // 타이머 리셋 플래그 설정
            }
        }

        private void SetTarget(eSkillTargetType targetType)
        {
            var targettingGuide = TargetStrategyFactory.CreateTargetStrategy(new TargettingStrategyParameter()
            {
                type = targetType,
                Caster = charAgent
            });
            cachedTargets = targettingGuide.GetTargets();
            if (cachedTargets == null)
            {
                Debug.Log("No targets to Encounter");
                return;
            }
            FinalTarget = CharUtil.GetNearestInList(charAgent, cachedTargets); // 무조건 cached 중 최근접 대상
            if (!FinalTarget)
            {
                Debug.Log("No target to Chase");
                return;
            }
            OnTargetSet?.Invoke(FinalTarget);
        }

        private void SetAction(eAttackMode attackMode)
        {
            SkillAIInfo info = charAgent.CharSKillInfo.GetInfoByMode(attackMode);
            SetTarget(info.TargetType);
            if (!FinalTarget)
            {
                Debug.LogWarning("타겟 도중 섬멸. 무효화되어 다음 주기에 타겟 할당합니다.");
                return;
            }
            
            //데이터 기반 사거리 설정 및 행동 결정
            int skillRange = info.Range;
            Vector3 displacement = FinalTarget.CharTransform.position - charAgent.CharTransform.position;
            charAgent.CharTransform.localScale = displacement.x > 0 ? new Vector3(1, 1, 1) : new Vector3(1, 1, -1);
            var distance = displacement.magnitude;

            // 사거리와 비교 후 이동 결정
            bool inRange = distance <= skillRange + SystemConst.TOLERANCE || skillRange == 0;
            if (inRange)
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(cachedTargets, attackMode));
            }
            else
            {
                charAgent.Nav.stoppingDistance = skillRange;
                charAgent.CharAction.CharMoveAction(new CharMoveParameter(FinalTarget));
            }
        }

        private void ComputeRestriction()
        {
            #region Default
            Targetable = true;
            Attackable = true;
            Movable = true;
            #endregion
            
            
        }
        
        public void Charm(CharBase target)
        {
            TargetForcedFix(target);
            Attackable = false;
            Targetable = false;
        }

        public void Stun(CharBase target)
        {
            Attackable = false;
            Targetable = false;
            Movable = false;
        }

        public void Taunt(CharBase target)
        {
            TargetForcedFix(target);
            Targetable = false;
            
        }
        
        public void TargetForcedFix(CharBase fixedTarget)
        {
            if (!fixedTarget) return;
            cachedTargets.Clear();
            cachedTargets.Add(fixedTarget);
        }
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
            SkillAIInfo info = charAgent.CharSKillInfo.GetInfoByMode(mode);
            SetTarget(info.TargetType);
            if (!FinalTarget)
            {
                Debug.LogWarning("타겟 도중 섬멸. 무효화되어 다음 주기에 타겟 할당합니다.");
                return;
            }
            
            //데이터 기반 사거리 설정 및 행동 결정
            int skillRange = info.Range;
            Vector3 displacement = FinalTarget.CharTransform.position - charAgent.CharTransform.position;
            Debug.Log(displacement);
            charAgent.CharTransform.localScale = displacement.z > 0 ? new Vector3(1, 1, 1) : new Vector3(1, 1, -1);
            var distance = displacement.magnitude;

            // 사거리와 비교 후 이동 결정
            bool inRange = distance <= SystemConst.GetWorldLength(skillRange) + SystemConst.TOLERANCE || skillRange == 0;
            if (inRange)
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(cachedTargets, mode));
            }
            else
            {
                Debug.LogError("스킬 사거리가 충분하지 않아, 스킬 사용이 불가합니다.\n" +
                               $"[스킬 사거리] {SystemConst.GetWorldLength(skillRange)} : , [현재 유닛 거리] : {distance}");
            }
        }
        
        public void TestSkillOnTarget(CharBase target, eAttackMode mode)
        {
            charAgent.CharAction.CharAttackAction(new CharAttackParameter(new List<CharBase> {target}, mode));
        }
        #endif
        #endregion
    }
}