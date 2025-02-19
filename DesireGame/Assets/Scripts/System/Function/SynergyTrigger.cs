namespace Client
{
    public class SynergyTrigger : FunctionBase
    {
        //*********���� ���� ����********//

        // �ó��� Ʈ���� ����� �ϴ� Function. ��� �׳� �Ⱦ�������. ��ü �Ҹ궧����.
        // [TODO] : �̰� �� �ʿ����� �ݵ�� �����ϰ� ����� ��.

        // ���� �ó����� ���� Ʈ����?
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

        // �� �� ������ ������ �Ǿ�� �ϴµ�
        // ���������� ���׳��� �ھƼ�
        // �Ŵ������� �׾�� ����. �ϸ� �ڱ��ڽ� killqueue�� ������ 

    }
}