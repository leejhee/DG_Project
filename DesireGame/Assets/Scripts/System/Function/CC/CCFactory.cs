namespace Client
{
    public static class CCFactory
    {
        public static CCBase CCGenerate(CCParameter param)
        {
            switch (param.ccType)
            {
                case SystemEnum.eCCType.SHRED:      return new CCShred(param);
                case SystemEnum.eCCType.SUNDER:     return new CCSunder(param);
                case SystemEnum.eCCType.CRIPPLE:    return new CCCripple(param);
                case SystemEnum.eCCType.STUN:       return new CCStun(param);
                case SystemEnum.eCCType.KNOCKBACK:  return new CCKnockBack(param);
                case SystemEnum.eCCType.CHARM:      return new CCCharm(param);
                case SystemEnum.eCCType.SILENCE:    return new CCSilence(param);
                case SystemEnum.eCCType.TAUNT:      return new CCTaunt(param);
                case SystemEnum.eCCType.WOUND:      return new CCWound(param);
            }
            return null;
        }
    }
}