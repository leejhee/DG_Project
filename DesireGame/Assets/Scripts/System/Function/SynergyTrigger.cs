namespace Client
{
    public class SynergyTrigger : FunctionBase
    {
        //*********구상 중인 구조********//

        // 시너지 트리깅 기능을 하는 Function
        // 캐릭터 활성화 시 매니저측 액션에 SynergySubscribe 구독.
        //
        // 


        public SynergyTrigger(BuffParameter buffParam) : base(buffParam)
        {

        }

        public override void RunFunction(bool StartExecution)
        {
            base.RunFunction(StartExecution);
            if (StartExecution)
            {

            }
            else
            {
            }
        }

        public void SynergySubscribe(SystemEnum.eSynergy invokerSynergy)
        {


        }

    }
}