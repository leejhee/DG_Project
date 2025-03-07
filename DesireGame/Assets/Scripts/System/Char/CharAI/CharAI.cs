using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class CharAI
    {
        public enum eAttackMode { None, Auto, Skill };

        private CharBase charAgent;

        private List<CharBase> cachedTargets;
        public CharBase finalTarget { get; private set; }// 우선 순위 계산의 최종 결과

        private PlayerState currentState; // 새로운 모드 변경 여부

        bool resetTimer = false;

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

                eAttackMode attackMode = SetAttackMode();

                SetStateByAttackMode(attackMode);
                SetAction(attackMode);

                yield return waitTime;
            }
        }
        

        /// <summary>
        /// 현재 상태에 따른 호출 간격 리턴
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        public float GetActionInterval(PlayerState newState)
        {
            float interval = 0;

            switch (newState)
            {
                case PlayerState.ATTACK:
                    interval = 1 / charAgent.CharStat.GetStat(eStats.NAS);
                    break;

                case PlayerState.MOVE:
                    interval = 1 / charAgent.CharStat.GetStat(eStats.NMOVE_SPEED); // TODO : 이동 거리 단위 기획에 물어보기
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
        public eAttackMode SetAttackMode()
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

        ///// <summary>
        ///// 최종 타겟 정하기
        ///// </summary>
        //public void SetFinalTarget(eAttackMode mode = eAttackMode.None)
        //{
        //    finalTarget = CharManager.Instance.GetNearestEnemy(charAgent);
        //    if (finalTarget != null)
        //        Debug.Log($"{charAgent.CharData.charName}{charAgent.GetID()}의 final target : {finalTarget.CharData.charName}");
        //}

        /// <summary>
        /// 상태 계산 및 행동 설정
        /// </summary>
        public void SetStateByAttackMode(eAttackMode attackMode)
        {
            switch (attackMode)
            {
                case eAttackMode.Auto:
                    ChangeState(PlayerState.ATTACK);
                    Debug.Log($"charAgent {charAgent.GetID()} 번 Auto mode로 전환");
                    break;

                case eAttackMode.Skill:
                    ChangeState(PlayerState.ATTACK);
                    Debug.Log($"charAgent {charAgent.GetID()} 번 Skill mode로 전환");
                    break;

                default:
                    ChangeState(PlayerState.IDLE);
                    Debug.Log($"charAgent {charAgent.GetID()} 번 Idle mode로 전환");
                    //SetFinalTarget();
                    break;
            }
        }

        public void ChangeState(PlayerState newState)
        {
            // 상태 변경 감지 시 
            if (currentState != newState)
            {
                Debug.Log($"{charAgent.CharData.charName}{charAgent.GetID()} : {currentState}에서 {newState}으로 상태 변경");
                currentState = newState;
                resetTimer = true; // 타이머 리셋 플래그 설정
            }
        }

        // 어택모드에 따른 스킬 인덱스 할당
        public long ReloadSkill(eAttackMode mode)
        {
            if (mode == eAttackMode.Auto)
            {
                return charAgent.CharData.skill1;
            }
            else if (mode == eAttackMode.Skill)
            {
                return charAgent.CharData.skill2;
            }
            else
            {
                Debug.Log("어택모드 할당 안되어 스킬 미사용");
                return SystemConst.NO_CONTENT;
            }
        }

        public void SetTarget(eSkillTargetType targetType)
        {
            var targettingGuide = TargetStrategyFactory.CreateTargetStrategy(new TargettingStrategyParameter()
            {
                type = targetType,
                Caster = charAgent
            });
            cachedTargets = targettingGuide.GetTargets();
            finalTarget = CharUtil.GetNearestInList(charAgent, cachedTargets); // 무조건 cached 중 최근접 대상
        }

        public void ExchangeMana(eAttackMode mode)
        {
            var stat = charAgent.CharStat;
            if (mode == eAttackMode.Auto)
            {
                if (stat.GetStat(eStats.MAX_MANA) != 0)
                    stat.GainMana(5, true);
            }
            else if (mode == eAttackMode.Skill)
            {
                stat.GainMana((int)stat.GetStat(eStats.MAX_MANA), false);
            }
        }

        public void SetAction(eAttackMode attackMode)
        {
            var skillIndex = ReloadSkill(attackMode);
            var skillData = DataManager.Instance.GetData<SkillData>(skillIndex);

            //데이터 기반 타겟 설정
            SetTarget(skillData.skillTarget);
            if (finalTarget == null)
            {
                Debug.LogWarning("타겟 도중 섬멸. 무효화되어 다음 프레임에 타겟 할당합니다.");
                return;
            }

            //데이터 기반 사거리 설정 및 행동 결정
            int skillRange = skillData.skillRange;
            var distance = Vector3.Distance(charAgent.CharTransform.position, finalTarget.CharTransform.position);
            var tolerance = 0.01f;
            // 사거리와 비교 후 이동 결정
            if (distance <= skillRange + tolerance || skillRange == 0)
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(cachedTargets, skillIndex));
                ExchangeMana(attackMode);

                Debug.Log($"캐릭터 {charAgent.CharData.charName} {charAgent.GetID()}의 스킬 {skillIndex} 사용");
            }
            else
            {
                charAgent.Nav.stoppingDistance = skillRange;
                charAgent.CharAction.CharMoveAction(new CharMoveParameter(finalTarget));
            }
        }

    }
}