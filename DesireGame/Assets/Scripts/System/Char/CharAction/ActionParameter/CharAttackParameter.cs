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

        // 하나만 할 수도 있으니까. 혹시 몰라서...
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
