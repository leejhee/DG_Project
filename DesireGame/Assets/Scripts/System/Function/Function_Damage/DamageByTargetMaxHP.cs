using UnityEngine;

namespace Client
{
    /// <summary> DAMAGE_BY_TARGET_MAXHP : Ÿ�� �ִ�ü���� {1}%�� ���ظ� ������. </summary>
    public class DamageByTargetMaxHP : FunctionBase
    {
        public DamageByTargetMaxHP(BuffParameter buffParam) : base(buffParam)
        {
        }

        // �̰� ����̶� �׳� Ʈ�������ڸ��� ��µ�, �����ϰ� update�� ���� ���.
        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                long rawRatio = _FunctionData.input1;
                float ratio = rawRatio / SystemConst.PER_TEN_THOUSAND;
                if (_TargetChar)
                {
                    var stat = _TargetChar.CharStat;
                    stat.ReceiveDamage(new DamageParameter()
                    {
                        damageType = _FunctionData.damageType,
                        rawDamage = stat.GetStat(SystemEnum.eStats.NMHP) * ratio,
                        penetration = _CastChar.CharStat.GetPenetration(_FunctionData.damageType)
                    });
                    Debug.Log($"�ִ�HP {ratio * 100} ��ʵ� �ߵ�.");
                }                
            }
        }
    }
}