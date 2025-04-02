using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class ArmorSynergyHeal : FunctionBase
    {
        private ConditionBase _statHPCondition = null;
        private long _tickDelta = 0;
        private float _secondTick = 0;
        public ArmorSynergyHeal(BuffParameter buffParam) : base(buffParam)
        {
            _tickDelta = (long)(_TargetChar.CharStat.GetStat(eStats.NMHP) * _FunctionData.input1 / SystemConst.PER_TEN_THOUSAND / 
                                (_FunctionData.time / SystemConst.PER_THOUSAND));
            
        }
        

        public override void Update(float delta)
        {
            base.Update(delta);
            if (_secondTick >= 1f)
            {
                _TargetChar.CharStat.ChangeStateByBuff(eStats.NHP, _tickDelta);
            }
            else
            {
                _secondTick += delta;
            }
            
        }
    }
}