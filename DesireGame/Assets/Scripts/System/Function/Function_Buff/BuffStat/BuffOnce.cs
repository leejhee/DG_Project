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
            delta = _TargetChar.CharStat.GetStatRaw(targetStat)
                * (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND);
        }
    }

    // 둘이 연관을 짓는게 맞을까...?

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

}