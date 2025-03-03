using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class CharAI
    {
        CharBase charAgent;
        CharBase finalTarget; // 우선 순위 계산의 최종 결과
        List<CharBase> cachedTargets;

        eAttackMode attackMode; // 공격 모드
        PlayerState currentState; // 새로운 모드 변경 여부
        
        bool resetTimer = false;
        float timer;
        float interval;

        public bool isAIRun { get; set; } = false; // 이 캐릭터는 현재 AI가 작동중입니다.

        public enum eAttackMode { None, Auto, Skill };
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

                SetState();
                yield return waitTime;
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

        /// <summary>
        /// 현재 상태에 따른 호출 간격 리턴
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        public float GetActionInterval(PlayerState newState)
        {
            float interval = 0;

            switch(newState)
            {
                case PlayerState.ATTACK:
                    interval = 1/charAgent.CharStat.GetStat(eStats.NAS); 
                    break;

                case PlayerState.MOVE:
                    interval = 1/charAgent.CharStat.GetStat(eStats.NMOVE_SPEED); // TODO : 이동 거리 단위 기획에 물어보기
                    break;
                
                default:
                    interval = Time.deltaTime;
                    break;
            }

            return interval;
        }

        /// <summary>
        /// 최종 타겟 정하기
        /// </summary>
        public void SetFinalTarget()
        {
            finalTarget = CharManager.Instance.GetNearestEnemy(charAgent);

            if (finalTarget != null)
                Debug.Log($"{charAgent.CharData.charName}{charAgent.GetID()}의 final target : {finalTarget.CharData.charName}");
        }

        /// <summary>
        /// 마나 양에 따라 공격 모드 판단
        /// </summary>
        /// <returns></returns>
        public eAttackMode SetAttackMode()
        {
            if (finalTarget == null) 
                return eAttackMode.None;

            // 스킬 사용 조건
            // 1: 최대 마나가 0 이상
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
        public void SetState()
        {
            long attackIndex = 0; // 평타 or 스킬 인덱스
            SkillBase skillBase;
            int skillRange = 0;

            // 공격 종류에 따라 사거리 기준 설정하기
            attackMode = SetAttackMode();
            switch (attackMode)
            {
                case eAttackMode.Auto:
                    attackIndex = charAgent.CharData.skill1;
                    ChangeState(PlayerState.ATTACK);
                    break;

                case eAttackMode.Skill:
                    attackIndex = charAgent.CharData.skill2;
                    ChangeState(PlayerState.ATTACK);
                    break;

                default:
                    ChangeState(PlayerState.IDLE);
                    SetFinalTarget();
                    return;
            }

            charAgent.CharSKillInfo.DicSkill.TryGetValue(attackIndex, out skillBase);
            skillRange = skillBase.NSkillRange;
            eSkillTargetType targetType = skillBase.TargetType;

            // transform 위치 기반 거리 측정
            float distanceSqr =  (charAgent.CharTransform.position - finalTarget.CharTransform.position).sqrMagnitude;

            // 내림 처리
            distanceSqr = Mathf.Floor(distanceSqr * 100) / 100;

            // 사거리와 비교 후 이동 결정
            if (distanceSqr <= Mathf.Pow(skillRange, 2))
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(finalTarget, attackIndex, targetType));
                Debug.Log($"캐릭터 {charAgent.CharData.charName} {charAgent.GetID()}의 스킬 {attackIndex} 사용");
            }
            else
            {
                Vector3 displacement = finalTarget.CharTransform.position - charAgent.CharTransform.position;
                Vector3 destination = charAgent.CharTransform.position + displacement.normalized * (displacement.magnitude - skillRange);

                charAgent.CharAction.CharMoveAction(new CharMoveParameter(destination));

            }
        }
    }
}