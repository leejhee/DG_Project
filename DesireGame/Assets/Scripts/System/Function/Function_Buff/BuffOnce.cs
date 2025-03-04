using UnityEngine;

namespace Client
{
    /// <summary>
    /// BUFF_ONCE : {statsType}�� {1}%��ŭ �� �� �����Ѵ�. {time}ms ���� �������.
    /// </summary>
    public class BuffOnce : StatBuffBase
    {
        public BuffOnce(BuffParameter buffParam) : base(buffParam)
        {

        }

        public override void ComputeDelta()
        {
            base.ComputeDelta();
            delta = _TargetChar.CharStat.GetStat(targetStat)
                * (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND);
        }
    }

    // ���� ������ ���°� ������...?

    /// <summary>
    /// BUFF_ONCE_BY_AD : �������� ���� ���ݷ��� {1}%��ŭ {statsType}�� �� �� ��ȭ
    /// </summary>
    public class BuffOnceByAD : StatBuffBase
    {
        public BuffOnceByAD(BuffParameter buffParam) : base(buffParam)
        {
            
        }

        public override void ComputeDelta()
        {
            base.ComputeDelta();
            delta = _CastChar.CharStat.GetStat(SystemEnum.eStats.NAD)
                * (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND);
        }

    }

}