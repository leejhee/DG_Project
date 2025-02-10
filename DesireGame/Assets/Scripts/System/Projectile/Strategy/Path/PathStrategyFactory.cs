namespace Client
{
    public struct PathStrategyParameter
    {
        public SystemEnum.eProjectilePathType type;
        public float Speed;
    }

    public static class PathStrategyFactory
    {
        public static IPathStrategy CreatePathStrategy(PathStrategyParameter param)
        {
            switch (param.type)
            {
                case SystemEnum.eProjectilePathType.STRAIGHT:
                    return new StraightPathStrategy(param.Speed);                   
                case SystemEnum.eProjectilePathType.TARGET_POSITION:
                case SystemEnum.eProjectilePathType.PINGPONG:
                case SystemEnum.eProjectilePathType.UNTIL_WALL:
                case SystemEnum.eProjectilePathType.WALL_BOUNCE:
                case SystemEnum.eProjectilePathType.EMax:
                default:
                    return null;
            }
        }
    }
}