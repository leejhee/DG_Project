using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CharAttackParameter : CharActionParameter
    {
        public List<CharBase> targetChar;
        public long skillIndex;

        public CharAttackParameter
            (
                List<CharBase> targetCharList, 
                long skillIndex
            )
        {
            this.targetChar = targetCharList;
            this.skillIndex = skillIndex;
        }

        // �ϳ��� �� ���� �����ϱ�. Ȥ�� ����...
        public CharAttackParameter
            (
                CharBase targetChar,
                long skillIndex
            )
        {
            this.targetChar = new List<CharBase>(){ targetChar };
            this.skillIndex = skillIndex;
        }



    }
}
