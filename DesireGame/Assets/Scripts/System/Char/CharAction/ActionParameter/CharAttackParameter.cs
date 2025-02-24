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

        // �ϳ��� �� ���� �����ϱ�. Ȥ�� ����...
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
