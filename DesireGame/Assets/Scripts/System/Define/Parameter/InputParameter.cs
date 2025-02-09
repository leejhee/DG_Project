
namespace Client
{
    public class InputParameter
    {
        public CharBase skillTarget;
        public CharBase skillCaster;

        // InputManager 사라지면 protected 전환 예정
        public InputParameter() { }

        public InputParameter
            (
                CharBase skillTarget, 
                CharBase skillCaster
            )
        {
            this.skillTarget = skillTarget;
            this.skillCaster = skillCaster;
        }
    }

    // 스탯과 적용비율을 설정하는 부분에서 다시 패킹하기 위해 사용
    public class StatPackParameter : InputParameter
    {
        public SystemEnum.eStats statOperand;
        public float percent;

        public StatPackParameter
            (
                InputParameter param,
                SystemEnum.eStats operand = SystemEnum.eStats.None,
                float ratio = 0f
            ): base(param.skillTarget, param.skillCaster)
        {
            statOperand = operand;
            percent = ratio;
        }
    }

    public static class ParameterExtensions
    {
        public static StatPackParameter ToStatPack(
            this InputParameter param,
            SystemEnum.eStats operand,
            float ratio)
        {
            return new StatPackParameter(param)
            {
                statOperand = operand,
                percent = ratio
            };
        }
    }
}
