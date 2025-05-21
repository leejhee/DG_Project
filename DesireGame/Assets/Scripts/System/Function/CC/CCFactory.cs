namespace Client
{
    public static class CCFactory
    {
        public static NegativeEffectBase CCGenerate(CCParameter param)
        {
            switch (param.ccType)
            {
                case SystemEnum.eCCType.SHRED:      return new NegativeEffectShred(param);
                case SystemEnum.eCCType.SUNDER:     return new NegativeEffectSunder(param);
                case SystemEnum.eCCType.CRIPPLE:    return new NegativeEffectCripple(param);
                case SystemEnum.eCCType.STUN:       return new NegativeEffectStun(param);
                case SystemEnum.eCCType.KNOCKBACK:  return new NegativeEffectKnockBack(param);
                case SystemEnum.eCCType.CHARM:      return new NegativeEffectCharm(param);
                case SystemEnum.eCCType.SILENCE:    return new NegativeEffectSilence(param);
                case SystemEnum.eCCType.TAUNT:      return new NegativeEffectTaunt(param);
                case SystemEnum.eCCType.WOUND:      return new NegativeEffectWound(param);
            }
            return null;
        }
    }
}