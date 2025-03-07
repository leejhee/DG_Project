using UnityEngine;

namespace Client
{
    public class ApplyCC : FunctionBase
    {
        public ApplyCC(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
        }

    }

    public abstract class CCBase
    {

    }

    public class LayOff : CCBase
    {

    }
    
}
