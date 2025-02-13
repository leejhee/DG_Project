using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class DamageByCasterAP : FunctionBase
    {
        public DamageByCasterAP(BuffParameter buffParam) : base(buffParam)
        {

        }
        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
            }
            else
            {
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            Debug.Log("시간만큼 떠야함. 즉 lifetime == 0이면 1번 떠야함");
        }

    }
}