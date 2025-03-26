using UnityEngine;

namespace Client
{
    public class ConditionCheck : FunctionBase
    {
        public ConditionCheck(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if(StartFunction)
            {
                if(_FunctionData.ConditionCheck == default)
                {
                    Debug.LogError("Condition Check�ε� ����� �ʱ�ȭ �ȵǾ�����. ������ üũ���");
                    return;
                }
                CheckFollowingCondition();
            }
        }
    }
}