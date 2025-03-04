using System;
using static Client.SystemEnum;

namespace Client
{
    /// <summary> ���� ����ڸ� ���� �� �����ϱ� ���� Action�� ���� </summary>
    public class StatBuffBase : FunctionBase
    {
        protected Action OnKillBuff;
        protected bool isTemporal;

        // ������ ������ ���̽��� ����
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