using System;
using static Client.SystemEnum;

namespace Client
{
    /// <summary> 스탯 버프와 관련된 Function </summary>
    public class StatBuffBase : FunctionBase
    {
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
            }
        }

        /// <remarks> <b>계산은 GetStatRaw를 사용해서 할 것</b> </remarks>
        public virtual void ComputeDelta() { }

        public virtual void ChangeStat(eStats stat, float delta)
        {
            if (stat == eStats.None || delta == default) return;
            _TargetChar.CharStat.ChangeStateByBuff(targetStat, (long)delta);
        }
    }
}