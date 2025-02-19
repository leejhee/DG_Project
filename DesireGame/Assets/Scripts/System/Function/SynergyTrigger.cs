namespace Client
{
    public class SynergyTrigger : FunctionBase
    {
        //*********구상 중인 구조********//

        // 시너지 트리깅 기능을 하는 Function. 얘는 그냥 안없어진다. 객체 소멸때까지.
        // [TODO] : 이게 왜 필요한지 반드시 납득하고 사용할 것.

        // 무슨 시너지에 대한 트리거?
        public SystemEnum.eSynergy mySynergy { get; private set; } 

        
        public SynergyTrigger(BuffParameter buffParam) : base(buffParam)
        {

        }

        public void InitTrigger(SystemEnum.eSynergy synergy)
        {
            mySynergy = synergy;
            SynergyManager.Instance.RegisterCharSynergy(_CastChar, mySynergy);
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

        // 팔 때 버프가 해제가 되어야 하는데
        // 버프에서도 안테나를 박아서
        // 매니저에서 죽어라 버프. 하면 자기자신 killqueue로 보내면 

    }
}