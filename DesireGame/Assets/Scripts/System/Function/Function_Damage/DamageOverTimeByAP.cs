
using System;

namespace Client
{
    public class DamageOverTimeByAP : FunctionBase
    {
        private float SecondTimer = 0;

        public DamageOverTimeByAP(BuffParameter buffParam) : base(buffParam)
        {
            Math.Clamp(SecondTimer, 0, 1);
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if(SecondTimer == 1)
            {
                SecondTimer = 0;
                _TargetChar.CharStat.ReceiveDamage(new DamageParameter()
                {
                    rawDamage = _CastChar.CharStat.GetStat(SystemEnum.eStats.NAP) *
                                (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND),
                    damageType = _FunctionData.damageType,
                    penetration = _CastChar.CharStat.GetPenetration(_FunctionData.damageType)
                });
            }
            else
            {
                SecondTimer += delta;
            }
        }
    }
}