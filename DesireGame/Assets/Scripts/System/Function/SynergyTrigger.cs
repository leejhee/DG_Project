namespace Client
{
    public class SynergyTrigger : FunctionBase
    {
        //*********���� ���� ����********//

        // �ó��� Ʈ���� ����� �ϴ� Function
        // ĳ���� Ȱ��ȭ �� �Ŵ����� �׼ǿ� SynergySubscribe ����.
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