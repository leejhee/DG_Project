using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CharNPC : CharBase
    {
        protected override SystemEnum.eCharType CharType => SystemEnum.eCharType.NPC;
        protected override void CharInit()
        {
            base.CharInit();
            CharManager.Instance.SetChar<CharNPC>(this);
        }

    }
}