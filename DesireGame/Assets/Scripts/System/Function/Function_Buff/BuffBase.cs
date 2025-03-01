using System;

namespace Client
{
    /// <summary> 버프 계산자를 종료 시 삭제하기 위해 Action을 가짐 </summary>
    /// <remarks> 반드시 버프 종료시 계산자를 dispose하도록 파생 클래스에서 구독할 것</remarks>
    public class BuffBase : FunctionBase
    {
        //어떤 버프이든 해당 단일 버프에 대한 계산자를 만들 때 여기다가 
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