using UnityEngine;

namespace Client
{
    public class CharAttackParameter : CharActionParameter
    {
        public CharBase targetChar;
        public long skillIndex;
        public SystemEnum.eSkillTargetType skillTargetType;

        public CharAttackParameter
            (
                CharBase targetChar, 
                long skillIndex,
                SystemEnum.eSkillTargetType skillTargetType
            )
        {
            this.targetChar = targetChar;
            this.skillIndex = skillIndex;
            this.skillTargetType = skillTargetType;
        }


    }
}
