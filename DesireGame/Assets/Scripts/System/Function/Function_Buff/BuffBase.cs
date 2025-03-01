using System;

namespace Client
{
    /// <summary> ���� ����ڸ� ���� �� �����ϱ� ���� Action�� ���� </summary>
    /// <remarks> �ݵ�� ���� ����� ����ڸ� dispose�ϵ��� �Ļ� Ŭ�������� ������ ��</remarks>
    public class BuffBase : FunctionBase
    {
        //� �����̵� �ش� ���� ������ ���� ����ڸ� ���� �� ����ٰ� 
        protected Action OnKillBuff;

        public BuffBase(BuffParameter buffParam) : base(buffParam)
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
                OnKillBuff.Invoke();
            }
        }
    }
}