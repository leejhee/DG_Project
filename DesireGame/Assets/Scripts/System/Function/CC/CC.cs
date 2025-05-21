namespace Client
{
    public class NegativeEffectKnockBack : NegativeEffectBase
    {
        public NegativeEffectKnockBack(CCParameter param) : base(param)
        {
        }
        public override void RunEffect()
        {
            base.RunEffect();
        }
    }

    public class NegativeEffectStun : NegativeEffectBase
    {
        public NegativeEffectStun(CCParameter param) : base(param)
        {
        }

        public override void RunEffect()
        {
            base.RunEffect();
        }

        public override void EndEffect()
        {
            base.EndEffect();
        }
    }

    public class NegativeEffectSunder : NegativeEffectBase
    {
        private readonly float delta;

        public NegativeEffectSunder(CCParameter param) : base(param)
        {
            delta = -0.4f * _Target.CharStat.GetStatRaw(SystemEnum.eStats.ARMOR);
        }

        public override void RunEffect()
        {
            base.RunEffect();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.ARMOR, (long)delta);
        }

        public override void EndEffect()
        {
            base.EndEffect();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.ARMOR, -(long)delta);
        }
    }

    public class NegativeEffectShred : NegativeEffectBase
    {
        private readonly float delta;

        public NegativeEffectShred(CCParameter param) : base(param)
        {
            delta = -0.4f * _Target.CharStat.GetStatRaw(SystemEnum.eStats.MAGIC_RESIST);
        }

        public override void RunEffect()
        {
            base.RunEffect();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.MAGIC_RESIST, (long)delta);
        }

        public override void EndEffect()
        {
            base.EndEffect();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.MAGIC_RESIST, -(long)delta);
        }
    }
    
    public class NegativeEffectCripple : NegativeEffectBase
    {
        private readonly float delta;
        
        public NegativeEffectCripple(CCParameter param) : base(param)
        {
            delta = -0.3f * _Target.CharStat.GetStatRaw(SystemEnum.eStats.MAGIC_RESIST);
        }
        
        public override void RunEffect()
        {
            base.RunEffect();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.NAS, (long)delta);
        }

        public override void EndEffect()
        {
            base.EndEffect();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.NAS, -(long)delta);
        }
    }

    public class NegativeEffectCharm : NegativeEffectBase
    {
        public NegativeEffectCharm(CCParameter param) : base(param)
        {
        }
    }
    
    public class NegativeEffectSilence : NegativeEffectBase
    {
        public NegativeEffectSilence(CCParameter param) : base(param)
        {
        }
    }
    
    public class NegativeEffectTaunt : NegativeEffectBase
    {
        public NegativeEffectTaunt(CCParameter param) : base(param)
        {
        }
    }

    public class NegativeEffectWound : NegativeEffectBase
    {
        public NegativeEffectWound(CCParameter param) : base(param)
        {
        }
    }
}