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
        List<CharBase> cachedTargets;

        eAttackMode attackMode; // ���� ���
        PlayerState currentState; // ���ο� ��� ���� ����
        
        bool resetTimer = false;
        float timer;
        float interval;

        public bool isAIRun { get; set; } = false; // �� ĳ���ʹ� ���� AI�� �۵����Դϴ�.

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
                Debug.Log($"{charAgent.CharData.charName}{charAgent.GetID()} : {currentState}���� {newState}���� ���� ����");
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
                    interval = Time.deltaTime;
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

            if (finalTarget != null)
                Debug.Log($"{charAgent.CharData.charName}{charAgent.GetID()}�� final target : {finalTarget.CharData.charName}");
        }

        /// <summary>
        /// ���� �翡 ���� ���� ��� �Ǵ�
        /// </summary>
        /// <returns></returns>
        public eAttackMode SetAttackMode()
        {
            if (finalTarget == null) 
                return eAttackMode.None;

            // ��ų ��� ����
            // 1: �ִ� ������ 0 �̻�
            // 2: ���� ���� >= �ִ� ����
            bool condition1 = charAgent.CharStat.GetStat(eStats.MAX_MANA) > 0;
            bool condition2 = charAgent.CharStat.GetStat(eStats.N_MANA) >= charAgent.CharStat.GetStat(eStats.MAX_MANA);

            if (condition1 && condition2) 
                return eAttackMode.Skill;
            else
                return eAttackMode.Auto;
        }

        /// <summary>
        /// ���� ��� �� �ൿ ����
        /// </summary>
        public void SetState()
        {
            long attackIndex = 0; // ��Ÿ or ��ų �ε���
            SkillBase skillBase;
            int skillRange = 0;

            // ���� ������ ���� ��Ÿ� ���� �����ϱ�
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

            // transform ��ġ ��� �Ÿ� ����
            float distanceSqr =  (charAgent.CharTransform.position - finalTarget.CharTransform.position).sqrMagnitude;

            // ���� ó��
            distanceSqr = Mathf.Floor(distanceSqr * 100) / 100;

            // ��Ÿ��� �� �� �̵� ����
            if (distanceSqr <= Mathf.Pow(skillRange, 2))
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(finalTarget, attackIndex, targetType));
                Debug.Log($"ĳ���� {charAgent.CharData.charName} {charAgent.GetID()}�� ��ų {attackIndex} ���");
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