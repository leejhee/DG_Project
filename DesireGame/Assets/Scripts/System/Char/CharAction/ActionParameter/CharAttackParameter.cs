using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CharAttackParameter : CharActionParameter
    {
        public List<CharBase> targetChar;
        public long skillIndex;
        public SystemEnum.eSkillTargetType skillTargetType;

        public CharAttackParameter
            (
                List<CharBase> targetCharList, 
                long skillIndex,
                SystemEnum.eSkillTargetType skillTargetType
            )
        {
            this.targetChar = targetCharList;
            this.skillIndex = skillIndex;
            this.skillTargetType = skillTargetType;
        }

        // 하나만 할 수도 있으니까. 혹시 몰라서...
        public CharAttackParameter
            (
                CharBase targetChar,
                long skillIndex,
                SystemEnum.eSkillTargetType skillTargetType
            )
        {
            this.targetChar = new List<CharBase>(){ targetChar };
            this.skillIndex = skillIndex;
            this.skillTargetType = skillTargetType;
        }



    }
}
