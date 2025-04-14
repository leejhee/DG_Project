using System;
using UnityEngine;
using UnityEngine.AI;
using static Client.CharAI;
using static Client.SystemEnum;

namespace Client
{
    public class CharAction
    {
        private CharBase Actor;

        private NavMeshAgent Nav;

        // 평타인지 스킬인지까지 따져서 발동하는 액션
        public Action<eAttackMode> OnAttackAction;

        public CharAction(CharBase actor)
        {
            Actor = actor;
            Nav = actor.Nav;
        }

        public void CharMoveAction(CharMoveParameter param)
        {
            var interval = (param.Destination - Actor.transform.position).sqrMagnitude;
            Nav.updateRotation = false;
            if(interval > 0.02f * 0.02f)
            {
                Nav.isStopped = false;
                Nav.SetDestination(param.Destination);
                Nav.speed = Actor.CharStat.GetStat(eStats.NMOVE_SPEED);
            }
            else
            {
                if (Nav.hasPath)
                {
                    Nav.ResetPath();
                }
                Nav.isStopped = true;
            }
        }

        public void CharAttackAction(CharAttackParameter param)
        {
            CharSKillInfo skillInfo = Actor.CharSKillInfo;
            //SkillInfo.PlaySkill(param.skillIndex,
            //    new SkillParameter(param.targetChar, Actor));
            skillInfo.PlayByMode(param.mode, new SkillParameter(param.targetChar, Actor));
            Actor.CharStat.GainMana(param.mode);
            OnAttackAction?.Invoke(param.mode);
        }

    }

}