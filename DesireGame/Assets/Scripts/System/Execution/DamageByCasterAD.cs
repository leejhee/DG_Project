using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class DamageByCasterAD : ExecutionBase
    {
        public DamageByCasterAD(BuffParameter buffParam) : base(buffParam)
        {

        }
        public override void RunExecution(bool StartExecution)
        {
            base.RunExecution(StartExecution);
            if (StartExecution)
            {
            }
            else
            {
            }
        }
    }
}