namespace Client
{
    /// <summary>
    /// AA로 마나를 획득할 때 현재 획득하는 마나의 {1}%만큼 마나를 추가로 획득한다.
    /// </summary>
    public class MagicSynergyManaBuff : FunctionBase
    {
        public MagicSynergyManaBuff(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.N_MANA_RESTORE_INCREASE, _FunctionData.input1);
            }
            else
            {
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.N_MANA_RESTORE_INCREASE, -_FunctionData.input1);
            }
        }
    }
}