using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// HP가 시전자 {statsType}의 {1}%만큼 {time}ms 에 걸쳐서 한 번 증가한다.
    /// </summary>
    public class ArmorSynergyHeal : FunctionBase
    {
        private readonly long _tickDelta = 0;
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
                _secondTick = 0;
            }
            else
            {
                _secondTick += delta;
            }
            
        }
    }
}