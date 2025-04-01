namespace Client
{
    public class RangedSynergyADBUFF : FunctionBase
    {
        private static int RangedEnemyKillCount = 0;

        public RangedSynergyADBUFF(BuffParameter buffParam) : base(buffParam)
        {
        }


        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if(StartFunction)
            {
                UpdateBuffByKillCount();
                

            }
            else
            {

            }
        }

        public void UpdateBuffByKillCount()
        {

        }

        public void KillBuffSubscribe()
        {
            //var enemies = CharManager.Instance.
        }
    }
}