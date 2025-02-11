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

        public enum eAttackMode { Auto, Skill };
        public CharAI(CharBase charAgent) 
        { 
            this.charAgent = charAgent;
        }

        /// <summary>
        /// 캐릭터 AI 행동 결정 타이머
        /// </summary>
        /// <param name="newState"></param>
        public IEnumerator UpdateAI(PlayerState newState)
        {
            // 스킬 사용하는 동안에는 타이머 작동 중단
            // 현재 상태가 공격or이동에 따라 주기 바꿔줘야 함
            // 상태 변경 시 기존 타이머 중단

            while (true)
            {
                // 상태 변경 감지 시 
                if (currentState != newState)
                {
                    currentState = newState;
                    resetTimer = true; // 타이머 리셋 플래그 설정
                }

                // 상태가 변경되었을 경우 타이머를 리셋
                if (resetTimer) 
                {
                    resetTimer = false;
                    float actionInterval = GetActionInterval(currentState);
                    WaitForSeconds waitTime = new WaitForSeconds(actionInterval);
                }

                yield return null;
            }
        }
        public float GetActionInterval(PlayerState newState)
        {
            float interval = 0;

            switch(newState)
            {
                case PlayerState.ATTACK:
                    interval = 0; 
                    break;

                case PlayerState.MOVE:
                    interval = 1;
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
                    attackIndex = charAgent.CharData.autoAttack;
                    break;

                case eAttackMode.Skill:
                    attackIndex = charAgent.CharData.skillAttack;
                    break;
            }
            Debug.Log($"캐릭터 {charAgent.CharData.charName}의 스킬 {attackIndex} 사용");

            charAgent.CharSKillInfo.DicSkill.TryGetValue(attackIndex, out skillBase);
            skillRange = skillBase.NSkillRange;

            // transform 위치 기반 거리 측정
            float distanceSqr =  (charAgent.CharTransform.position - finalTarget.CharTransform.position).sqrMagnitude;
            
            // 사거리와 비교 후 이동 결정
            if (distanceSqr <= skillRange)
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(finalTarget, attackIndex));
            }
            else
            {
                charAgent.CharAction.CharMoveAction(new CharMoveParameter(finalTarget));
            }
        }
    }
}