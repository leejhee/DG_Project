namespace Client
{
    public class TeleportToAllyRear : FunctionBase
    {
        public TeleportToAllyRear(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if(StartFunction)
            {
                //여기서 먼곳 찾아서 이동시키기.
            }
        }
    }
}