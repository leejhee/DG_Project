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
            if(interval > 0.1f)
            {
                Nav.isStopped = false;
                Nav.SetDestination(param.Destination);
            }
            else
            {
                Nav.isStopped = true;
            }
        }

        public void CharAttackAction(CharAttackParameter param)
        {
            var SkillInfo = Actor.CharSKillInfo;
            if(SkillInfo.DicSkill.TryGetValue(param.skillIndex, out var skill))
            {
                skill.PlaySkill(new InputParameter());
            }
            else
            {
                Debug.LogError($"{param.skillIndex} �׷� ��ų�� ���ٰ� �Ѵ�.");
            }
        }
    }

}