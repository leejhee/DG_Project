using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CharMonster : CharBase
    {
        protected override SystemEnum.eCharType CharType => SystemEnum.eCharType.Monster;
        protected override void CharInit()
        {
            base.CharInit();
            CharManager.Instance.SetChar<CharMonster>(this);
        }

        public void Patrol()
        {
            //Patrol
        }

    }
}