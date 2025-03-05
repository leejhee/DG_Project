using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    public class CharAction
    {
        CharBase Actor;

        NavMeshAgent Nav;

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
                Nav.speed = Actor.CharStat.GetStat(SystemEnum.eStats.NMOVE_SPEED);
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
            var SkillInfo = Actor.CharSKillInfo;
            SkillInfo.PlaySkill(param.skillIndex,
                new SkillParameter(param.targetChar, Actor, param.skillTargetType));         
        }
    }

}