using UnityEngine;

namespace Client
{
    public class CharAttackParameter : CharActionParameter
    {
        public CharBase targetChar;
        public long skillIndex;

        public CharAttackParameter
            (
                CharBase targetChar, 
                long skillIndex               
            )
        {
            this.targetChar = targetChar;
            this.skillIndex = skillIndex;
        }


    }
}
