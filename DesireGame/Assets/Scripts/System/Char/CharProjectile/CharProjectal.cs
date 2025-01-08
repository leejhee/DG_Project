using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Client
{
    public class CharProjectal : CharBase
    {
        [SerializeField]
        protected float gravity;

        protected Vector3 Origin;
       
        protected override SystemEnum.eCharType CharType => SystemEnum.eCharType.Projectal;
        protected override void CharInit()
        {
            //base.CharInit();
            //CharManager.Instance.SetChar<CharProjectal>(this);

            Origin = transform.position;
        }

        
    }
}