namespace Client
{
    public struct TargettingStrategyParameter
    {
        public SystemEnum.eProjectileTargetType type;
        public CharBase Target;
    }

    public static class TargetStrategyFactory
    {
        public static ITargettingStrategy CreateTargetStrategy(TargettingStrategyParameter param)
        {
            switch(param.type)
            {
                case SystemEnum.eProjectileTargetType.NEAR_TARGET:
                case SystemEnum.eProjectileTargetType.CURRENT_TARGET:
                case SystemEnum.eProjectileTargetType.FARTHEST_TARGET:
                    return new TargetedStrategy(param.Target);
                default:
                    return null;
            }
        }
    }
}