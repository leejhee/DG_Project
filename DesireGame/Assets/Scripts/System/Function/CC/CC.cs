namespace Client
{
    public class CCKnockBack : CCBase
    {
        public CCKnockBack(CCParameter param) : base(param)
        {
        }
        public override void RunCC()
        {
            base.RunCC();
        }
    }

    public class CCStun : CCBase
    {
        public CCStun(CCParameter param) : base(param)
        {
        }

        public override void RunCC()
        {
            base.RunCC();
        }

        public override void EndCC()
        {
            base.EndCC();
        }
    }

    public class CCSunder : CCBase
    {
        private readonly float delta;

        public CCSunder(CCParameter param) : base(param)
        {
            delta = -0.4f * _Target.CharStat.GetStatRaw(SystemEnum.eStats.ARMOR);
        }

        public override void RunCC()
        {
            base.RunCC();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.ARMOR, (long)delta);
        }

        public override void EndCC()
        {
            base.EndCC();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.ARMOR, -(long)delta);
        }
    }

    public class CCShred : CCBase
    {
        private readonly float delta;

        public CCShred(CCParameter param) : base(param)
        {
            delta = -0.4f * _Target.CharStat.GetStatRaw(SystemEnum.eStats.MAGIC_RESIST);
        }

        public override void RunCC()
        {
            base.RunCC();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.MAGIC_RESIST, (long)delta);
        }

        public override void EndCC()
        {
            base.EndCC();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.MAGIC_RESIST, -(long)delta);
        }
    }
    
    public class CCCripple : CCBase
    {
        private readonly float delta;
        
        public CCCripple(CCParameter param) : base(param)
        {
            delta = -0.3f * _Target.CharStat.GetStatRaw(SystemEnum.eStats.MAGIC_RESIST);
        }
        
        public override void RunCC()
        {
            base.RunCC();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.NAS, (long)delta);
        }

        public override void EndCC()
        {
            base.EndCC();
            _Target.CharStat.ChangeStateByBuff(SystemEnum.eStats.NAS, -(long)delta);
        }
    }

    public class CCCharm : CCBase
    {
        public CCCharm(CCParameter param) : base(param)
        {
        }
    }
    
    public class CCSilence : CCBase
    {
        public CCSilence(CCParameter param) : base(param)
        {
        }
    }
    
    public class CCTaunt : CCBase
    {
        public CCTaunt(CCParameter param) : base(param)
        {
        }
    }

    public class CCWound : CCBase
    {
        public CCWound(CCParameter param) : base(param)
        {
        }
    }
}