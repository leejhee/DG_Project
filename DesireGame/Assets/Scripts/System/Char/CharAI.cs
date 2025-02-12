using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class CharAI
    {
        CharBase charAgent;
        CharBase finalTarget; // �켱 ���� ����� ���� ���
        eAttackMode attackMode; // ���� ���
        PlayerState currentState; // ���ο� ��� ���� ����
        
        bool resetTimer = false;
        float timer;
        float interval;

        public enum eAttackMode { None, Auto, Skill };
        public CharAI(CharBase charAgent) 
        { 
            this.charAgent = charAgent;
        }

        /// <summary>
        /// ĳ���� AI �ൿ ���� Ÿ�̸�
        /// </summary>
        /// <param name="newState"></param>
        public IEnumerator UpdateAI(PlayerState initialState)
        {
            // ��ų ����ϴ� ���ȿ��� Ÿ�̸� �۵� �ߴ�
            // ���� ���°� ����or�̵��� ���� SetState() ȣ�� �ֱ� �ٲ���� ��
            // ���� ���� �� ���� Ÿ�̸� �ߴ�

            currentState = initialState;
            float actionInterval = GetActionInterval(currentState);
            WaitForSeconds waitTime = new WaitForSeconds(actionInterval);

            while (true)
            {
                // ���°� ����Ǿ��� ��� Ÿ�̸Ӹ� ����
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
            // ���� ���� ���� �� 
            if (currentState != newState)
            {
                Debug.Log($"{currentState}���� {newState}���� ���� ����");
                currentState = newState;
                resetTimer = true; // Ÿ�̸� ���� �÷��� ����
            }
        }

        /// <summary>
        /// ���� ���¿� ���� ȣ�� ���� ����
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
                    interval = 1/charAgent.CharStat.GetStat(eStats.NMOVE_SPEED); // TODO : �̵� �Ÿ� ���� ��ȹ�� �����
                    break;
                
                default:
                    interval = Time.deltaTime; // SetState() �ٷ� ȣ�� �� �� �ֵ���
                    break;
            }

            return interval;
        }

        /// <summary>
        /// ���� Ÿ�� ���ϱ�
        /// </summary>
        public void SetFinalTarget()
        {
            finalTarget = CharManager.Instance.GetNearestEnemy(charAgent);
            Debug.Log($"final target : {finalTarget.CharData.charName}");
        }

        /// <summary>
        /// ���� �翡 ���� ���� ��� �Ǵ�
        /// </summary>
        /// <returns></returns>
        public eAttackMode SetAttackMode()
        {
            // ��� ��Ȳ�� None�� �Ǿ�� ����?
            // 1. finalTarget ���� ��

            if (finalTarget == null)
            {
                SetFinalTarget();
                return eAttackMode.None;
            }

            // ��ų ��� ���� : ���� ���� >= �ִ� ����
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
        /// ���� ��� �� �ൿ ����
        /// </summary>
        public void SetState()
        {
            long attackIndex = 0; // ��Ÿ or ��ų �ε���

            SkillBase skillBase; // ��ų ����
            int skillRange = 0; // ��ų ��Ÿ� �޾ƿ�


            // � ������ ������ ���� ��Ÿ� ���� �����ϱ�
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
                    return; // �ٽ� Ÿ�� ã�Ƽ� AI ���� �� �ֵ���
            }

            charAgent.CharSKillInfo.DicSkill.TryGetValue(attackIndex, out skillBase);
            skillRange = skillBase.NSkillRange;

            // transform ��ġ ��� �Ÿ� ����
            float distanceSqr =  (charAgent.CharTransform.position - finalTarget.CharTransform.position).sqrMagnitude;
            
            // ��Ÿ��� �� �� �̵� ����
            if (distanceSqr <= (skillRange^2))
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(finalTarget, attackIndex));
                Debug.Log($"ĳ���� {charAgent.CharData.charName}�� ��ų {attackIndex} ���");
            }
            else
            {
                Debug.Log($"{charAgent.CharData.charName}�� {finalTarget.CharData.charName}������ �̵��մϴ�");

                // TODO: Vector3�� �Ķ���ͷ� ����
                // Ÿ���� �����Ǻ��� ��Ÿ���ŭ ������ �־�� �Ѵ�
                Vector3 displacement = finalTarget.CharTransform.position - charAgent.CharTransform.position;
                Vector3 destination = charAgent.CharTransform.position + displacement.normalized * (displacement.magnitude - skillRange);

                charAgent.CharAction.CharMoveAction(new CharMoveParameter(destination));

            }
        }
    }
}