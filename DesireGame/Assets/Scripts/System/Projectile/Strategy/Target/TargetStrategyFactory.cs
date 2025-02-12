namespace Client
{
    public struct TargettingStrategyParameter
    {
        public SystemEnum.eSkillTargetType type;
        public CharBase Target;
    }

    public static class TargetStrategyFactory
    {
        public static ITargettingStrategy CreateTargetStrategy(TargettingStrategyParameter param)
        {
            switch(param.type)
            {
                case SystemEnum.eSkillTargetType.NEAR_ENEMY:
                case SystemEnum.eSkillTargetType.CURRENT_ENEMY:
                case SystemEnum.eSkillTargetType.FARTHEST_ENEMY:
                    return new TargetedStrategy(param.Target);
                default:
                    return null;
            }
        }
    }
}