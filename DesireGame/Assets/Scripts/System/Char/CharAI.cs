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

        public enum eAttackMode { Auto, Skill };
        public CharAI(CharBase charAgent) 
        { 
            this.charAgent = charAgent;
        }

        /// <summary>
        /// ĳ���� AI �ൿ ���� Ÿ�̸�
        /// </summary>
        /// <param name="newState"></param>
        public IEnumerator UpdateAI(PlayerState newState)
        {
            // ��ų ����ϴ� ���ȿ��� Ÿ�̸� �۵� �ߴ�
            // ���� ���°� ����or�̵��� ���� �ֱ� �ٲ���� ��
            // ���� ���� �� ���� Ÿ�̸� �ߴ�

            while (true)
            {
                // ���� ���� ���� �� 
                if (currentState != newState)
                {
                    currentState = newState;
                    resetTimer = true; // Ÿ�̸� ���� �÷��� ����
                }

                // ���°� ����Ǿ��� ��� Ÿ�̸Ӹ� ����
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
                    attackIndex = charAgent.CharData.autoAttack;
                    break;

                case eAttackMode.Skill:
                    attackIndex = charAgent.CharData.skillAttack;
                    break;
            }
            Debug.Log($"ĳ���� {charAgent.CharData.charName}�� ��ų {attackIndex} ���");

            charAgent.CharSKillInfo.DicSkill.TryGetValue(attackIndex, out skillBase);
            skillRange = skillBase.NSkillRange;

            // transform ��ġ ��� �Ÿ� ����
            float distanceSqr =  (charAgent.CharTransform.position - finalTarget.CharTransform.position).sqrMagnitude;
            
            // ��Ÿ��� �� �� �̵� ����
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