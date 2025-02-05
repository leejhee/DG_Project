using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class CharAI
    {
        /* 필요한 것
        1. 위치
        2. 상태
        ------------
        1. 우선 순위 계산 + 타겟 설정
        2. 상태 계산
        */

        CharBase charAgent;
        CharBase finalTarget; // 우선 순위 계산의 최종 결과

        public CharAI(CharBase charAgent) 
        { 
            this.charAgent = charAgent;
        }

        /// <summary>
        /// 최종 타겟 정하기
        /// </summary>
        public void SetFinalTarget()
        {
            finalTarget = CharManager.Instance.GetNearestEnemy(charAgent);
        }

        /// <summary>
        /// 상태 계산 및 행동 설정
        /// </summary>
        public void SetState()
        {
            // 이동 해야하는지, 공격 해야 하는지 체크

            // transform 위치 기반 거리 측정
            float distanceSqr =  (charAgent.CharTransform.position - finalTarget.CharTransform.position).sqrMagnitude;

            // 사거리보다 멀리 있는 경우 이동
                // 어디로 이동?

            // 사거리 내에 있는 경우 공격 or 스킬 체크

        }

        public void SetAttackMode()
        {
            // 공격 or 스킬 체크
            // 스킬 사용 조건 : 현재 마나 >= 최대 마나

        }

    }
}