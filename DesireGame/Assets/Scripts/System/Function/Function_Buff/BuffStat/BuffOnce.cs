using UnityEngine;

namespace Client
{
    /// <summary>
    /// BUFF_ONCE : {statsType}가 {1}%만큼 한 번 증가한다. {time}ms 이후 사라진다.
    /// </summary>
    public class BuffOnce : StatBuffBase
    {
        public BuffOnce(BuffParameter buffParam) : base(buffParam)
        {
            isTemporal = true;
        }

        public override void ComputeDelta()
        {
            base.ComputeDelta();
            //delta = _TargetChar.CharStat.GetStatRaw(targetStat)
            //    * (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND);
            delta = _FunctionData.input1 / SystemConst.PER_TEN_THOUSAND;
        }

        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                CachedModifier = new StatModifier
                    (targetStat, SystemEnum.eOpCode.Mul, SystemEnum.eModifierRoot.Buff, delta);
                _TargetChar.CharStat.AddStatModification(CachedModifier);
            }
        }
    }


    /// <summary>
    /// BUFF_ONCE_BY_AD : 시전자의 현재 공격력의 {1}%만큼 {statsType}가 한 번 변화
    /// </summary>
    public class BuffOnceByAD : StatBuffBase
    {
        public BuffOnceByAD(BuffParameter buffParam) : base(buffParam)
        {
            isTemporal = false;
        }

        public override void ComputeDelta()
        {
            base.ComputeDelta();
            delta = _CastChar.CharStat.GetStatRaw(SystemEnum.eStats.NAD)
                * (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND);
        }
        
    }

    public class BuffOnceByAP : StatBuffBase
    {
        public BuffOnceByAP(BuffParameter buffParam) : base(buffParam)
        {
            isTemporal = false;
        }

        public override void ComputeDelta()
        {
            base.ComputeDelta();
            delta = _CastChar.CharStat.GetStatRaw(SystemEnum.eStats.NAP)
                * (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND);
        }
        
    }

    public class BuffOncePlus : StatBuffBase
    {
        public BuffOncePlus(BuffParameter buffParam) : base(buffParam)
        {
            
        }

        public override void ComputeDelta()
        {
            base.ComputeDelta();
            delta = _FunctionData.input1;
        }
    }
    
}