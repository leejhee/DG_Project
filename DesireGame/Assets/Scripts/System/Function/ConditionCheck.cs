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
                    Debug.LogError("Condition Check인데 제대로 초기화 안되어있음. 데이터 체크요망");
                    return;
                }
                CheckFollowingCondition();
            }
        }
    }
}