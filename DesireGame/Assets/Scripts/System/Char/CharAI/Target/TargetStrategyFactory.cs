namespace Client
{
    public struct TargettingStrategyParameter
    {
        public SystemEnum.eSkillTargetType type;
        public CharBase Caster;
    }

    public static class TargetStrategyFactory
    {
        public static TargettingStrategyBase CreateTargetStrategy(TargettingStrategyParameter param)
        {
            switch (param.type)
            {
                case SystemEnum.eSkillTargetType.CURRENT_ENEMY:
                case SystemEnum.eSkillTargetType.CURRENT_NEAR1_ENEMY:
                    return new CurrentEnemyTargetStrategy(param);
                case SystemEnum.eSkillTargetType.NEAR_ENEMY:
                    return new NearEnemyTargetStrategy(param);
                case SystemEnum.eSkillTargetType.NEAR_ENEMY_2:
                    return new NearEnemy2TargetStrategy(param);
                case SystemEnum.eSkillTargetType.NEAR_ENEMY_3:
                    return new NearEnemy3TargetStrategy(param);
                case SystemEnum.eSkillTargetType.FARTHEST_ENEMY:
                    return new FarthestEnemyTargetStrategy(param);
                case SystemEnum.eSkillTargetType.FARTHEST_ENEMY_2:
                    return new FarthestEnemy2TargetStrategy(param);
                case SystemEnum.eSkillTargetType.LOW_HP_ENEMY:
                case SystemEnum.eSkillTargetType.LOW_HP_ALLY:
                case SystemEnum.eSkillTargetType.SELF:
                    return new SelfTargetStrategy(param);
                case SystemEnum.eSkillTargetType.NEAR_ALLY_3:
                case SystemEnum.eSkillTargetType.EVERY_ENEMY:
                    return new EveryEnemyTargetStrategy(param);
                case SystemEnum.eSkillTargetType.EVERY_ALLY:
                    return new EveryAllyTargetStrategy(param);
                case SystemEnum.eSkillTargetType.CONTACT_ENEMY:                    
                default:
                    return null;
            }
        }
    }
}