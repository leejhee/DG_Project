
namespace Client
{
    // [TODO] : 복수 타겟 작업할 때 skillTarget을 List<CharBase>로 변경할 것.
    public class InputParameter
    {
        public CharBase skillTarget;
        public CharBase skillCaster;
        public SystemEnum.eSkillTargetType skillTargetType;

        // InputManager 사라지면 protected 전환 예정
        public InputParameter() { }

        public InputParameter
            (
                CharBase skillTarget, 
                CharBase skillCaster,
                SystemEnum.eSkillTargetType skillTargetType = 
                    SystemEnum.eSkillTargetType.None
            )
        {
            this.skillTarget = skillTarget;
            this.skillCaster = skillCaster;
            this.skillTargetType = skillTargetType;
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
            ): base(param.skillTarget, param.skillCaster, param.skillTargetType)
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
