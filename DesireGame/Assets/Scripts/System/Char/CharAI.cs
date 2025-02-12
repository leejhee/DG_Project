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
        eAttackMode attackMode; // 공격 모드
        PlayerState currentState; // 새로운 모드 변경 여부
        
        bool resetTimer = false;
        float timer;
        float interval;

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
                Debug.Log($"{currentState}에서 {newState}으로 상태 변경");
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
                    interval = Time.deltaTime; // SetState() 바로 호출 할 수 있도록
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
            Debug.Log($"final target : {finalTarget.CharData.charName}");
        }

        /// <summary>
        /// 마나 양에 따라 공격 모드 판단
        /// </summary>
        /// <returns></returns>
        public eAttackMode SetAttackMode()
        {
            // 어느 상황에 None이 되어야 하지?
            // 1. finalTarget 없을 때

            if (finalTarget == null)
            {
                SetFinalTarget();
                return eAttackMode.None;
            }

            // 스킬 사용 조건 : 현재 마나 >= 최대 마나
            if (charAgent.CharStat.GetStat(eStats.N_MANA) >= charAgent.CharStat.GetStat(eStats.MAX_MANA))
            {
                return eAttackMode.Skill;
            }
            else
            {
                return eAttackMode.Auto;
            }
        }

        /// <summary>
        /// 상태 계산 및 행동 설정
        /// </summary>
        public void SetState()
        {
            long attackIndex = 0; // 평타 or 스킬 인덱스

            SkillBase skillBase; // 스킬 정보
            int skillRange = 0; // 스킬 사거리 받아옴


            // 어떤 공격을 할지에 따라 사거리 기준 설정하기
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
                    return; // 다시 타겟 찾아서 AI 돌릴 수 있도록
            }

            charAgent.CharSKillInfo.DicSkill.TryGetValue(attackIndex, out skillBase);
            skillRange = skillBase.NSkillRange;

            // transform 위치 기반 거리 측정
            float distanceSqr =  (charAgent.CharTransform.position - finalTarget.CharTransform.position).sqrMagnitude;
            
            // 사거리와 비교 후 이동 결정
            if (distanceSqr <= (skillRange^2))
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(finalTarget, attackIndex));
                Debug.Log($"캐릭터 {charAgent.CharData.charName}의 스킬 {attackIndex} 사용");
            }
            else
            {
                Debug.Log($"{charAgent.CharData.charName}가 {finalTarget.CharData.charName}쪽으로 이동합니다");

                // TODO: Vector3인 파라미터로 쓰기
                // 타겟의 포지션보다 사거리만큼 떨어져 있어야 한다
                Vector3 displacement = finalTarget.CharTransform.position - charAgent.CharTransform.position;
                Vector3 destination = charAgent.CharTransform.position + displacement.normalized * (displacement.magnitude - skillRange);

                charAgent.CharAction.CharMoveAction(new CharMoveParameter(destination));

            }
        }
    }
}