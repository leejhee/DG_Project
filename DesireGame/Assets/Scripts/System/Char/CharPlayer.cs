using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CharPlayer : CharBase
    {
        protected override SystemEnum.eCharType CharType => SystemEnum.eCharType.Player;
        protected override void CharInit()
        {
            base.CharInit();
            CharManager.Instance.SetChar<CharPlayer>(this);
        }
    }
}