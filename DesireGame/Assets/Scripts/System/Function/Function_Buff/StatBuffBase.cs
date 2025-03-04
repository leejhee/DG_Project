using System;
using static Client.SystemEnum;

namespace Client
{
    /// <summary> 버프 계산자를 종료 시 삭제하기 위해 Action을 가짐 </summary>
    public class StatBuffBase : FunctionBase
    {
        protected Action OnKillBuff;
        protected bool isTemporal;

        // 데이터 상으로 베이스인 스탯
        protected eStats targetStat;
        protected float delta = 0;
      
        public StatBuffBase(BuffParameter buffParam) : base(buffParam)
        {
            targetStat = _FunctionData.statsType;
            isTemporal = (_LifeTime != 0);
        }


        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                ComputeDelta();
                ChangeStat(targetStat, delta);
            }
            else
            {
                if (isTemporal)
                {
                    ChangeStat(targetStat, -delta);                    
                }
                OnKillBuff?.Invoke();
            }
        }

        public virtual void ComputeDelta() { }

        public virtual void ChangeStat(eStats stat, float delta)
        {
            if (stat == eStats.None || delta == default) return;
            _TargetChar.CharStat.ChangeStateByBuff(targetStat, (long)delta);
        }
    }
}