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

        private CharBase charAgent;

        private List<CharBase> cachedTargets;

        private PlayerState currentState; // ���ο� ��� ���� ����

        private bool resetTimer = false;
        
        public Action<CharBase> OnTargetSet;    
        
        public CharBase FinalTarget { get; private set; }// �켱 ���� ����� ���� ���
        public bool isAIRun { get; set; } = false; // �� ĳ���ʹ� ���� AI�� �۵����Դϴ�.

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

                eAttackMode attackMode = SetAttackMode();

                SetStateByAttackMode(attackMode);
                SetAction(attackMode);

                yield return waitTime;
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

            switch (newState)
            {
                case PlayerState.ATTACK:
                    interval = 1 / charAgent.CharStat.GetStat(eStats.NAS);
                    break;

                case PlayerState.MOVE:
                    interval = 1 / charAgent.CharStat.GetStat(eStats.NMOVE_SPEED); // TODO : �̵� �Ÿ� ���� ��ȹ�� �����
                    break;

                default:
                    interval = Time.deltaTime;
                    break;
            }

            return interval;
        }

        /// <summary>
        /// ���� �翡 ���� ���� ��� �Ǵ�
        /// </summary>
        /// <returns></returns>
        public eAttackMode SetAttackMode()
        {
            // ��ų ��� ����
            // 1: �ִ� ������ 0���� ũ��
            // 2: ���� ���� >= �ִ� ����
            bool condition1 = charAgent.CharStat.GetStat(eStats.MAX_MANA) > 0;
            bool condition2 = charAgent.CharStat.GetStat(eStats.N_MANA) >= charAgent.CharStat.GetStat(eStats.MAX_MANA);

            if (condition1 && condition2)
                return eAttackMode.Skill;
            else
                return eAttackMode.Auto;
        }

        ///// <summary>
        ///// ���� Ÿ�� ���ϱ�
        ///// </summary>
        //public void SetFinalTarget(eAttackMode mode = eAttackMode.None)
        //{
        //    finalTarget = CharManager.Instance.GetNearestEnemy(charAgent);
        //    if (finalTarget != null)
        //        Debug.Log($"{charAgent.CharData.charName}{charAgent.GetID()}�� final target : {finalTarget.CharData.charName}");
        //}

        /// <summary>
        /// ���� ��� �� �ൿ ����
        /// </summary>
        public void SetStateByAttackMode(eAttackMode attackMode)
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

        // ���ø�忡 ���� ��ų �ε��� �Ҵ�
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
                Debug.Log("���ø�� �Ҵ� �ȵǾ� ��ų �̻��");
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
            FinalTarget = CharUtil.GetNearestInList(charAgent, cachedTargets); // ������ cached �� �ֱ��� ���
            OnTargetSet?.Invoke(FinalTarget);
        }

        public void SetAction(eAttackMode attackMode)
        {
            var skillIndex = ReloadSkill(attackMode);
            var skill = charAgent.CharSKillInfo.DicSkill[skillIndex];
            //������ ��� Ÿ�� ����
            SetTarget(skill.TargetType);
            if (FinalTarget == null)
            {
                Debug.LogWarning("Ÿ�� ���� ����. ��ȿȭ�Ǿ� ���� �����ӿ� Ÿ�� �Ҵ��մϴ�.");
                return;
            }

            //������ ��� ��Ÿ� ���� �� �ൿ ����
            int skillRange = skill.NSkillRange;
            var distance = Vector3.Distance(charAgent.CharTransform.position, FinalTarget.CharTransform.position);
            var tolerance = 0.01f;

            // ��Ÿ��� �� �� �̵� ����
            bool inRange = distance <= skillRange + tolerance || skillRange == 0;
            if(charAgent.Index == 200)
                Debug.Log(skillRange);
            if (inRange)
            {
                charAgent.CharAction.CharAttackAction(new CharAttackParameter(cachedTargets, skillIndex, attackMode));
                Debug.Log($"ĳ���� {charAgent.CharData.charName} {charAgent.GetID()}�� ��ų {skillIndex} ���");
            }
            else
            {
                charAgent.Nav.stoppingDistance = skillRange;
                charAgent.CharAction.CharMoveAction(new CharMoveParameter(FinalTarget));
            }
        }

    }
}