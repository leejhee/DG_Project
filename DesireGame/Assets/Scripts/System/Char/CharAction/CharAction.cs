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
            var speed = Actor.CharStat.GetStat(SystemEnum.eState.Speed);
            var direction = param.Destination - Actor.transform.position;
            if(direction.sqrMagnitude > 0.1f)
            {
                Nav.isStopped = false; ;
                Nav.Move(speed * Time.deltaTime * direction.normalized);
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
                Debug.LogError($"{param.skillIndex} 그런 스킬은 없다고 한다.");
            }
        }
    }

}