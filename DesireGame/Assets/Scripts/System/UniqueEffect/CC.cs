namespace Client
{
    public class EffectKnockBack : EffectBase
    {
        public EffectKnockBack(EffectParameter param) : base(param)
        {
        }
        public override void RunEffect()
        {
            base.RunEffect();
        }
    }

    public class EffectStun : EffectBase
    {
        public EffectStun(EffectParameter param) : base(param)
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
        
        public void Update()
        {
            throw new System.NotImplementedException();
        }
        
    }

    
    /// <summary>
    /// CHARM : 매혹. 느린 속도로 시전자한테 접근
    /// </summary>
    public class EffectCharm : EffectBase
    {
        public EffectCharm(EffectParameter param) : base(param)
        {}

        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharAI.TargetForcedFix(Caster);
            
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
            
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
    
    /// <summary>
    /// SILENCE : 침묵. 마나 회복 불가
    /// </summary>
    public class EffectSilence : EffectBase
    {
        public EffectSilence(EffectParameter param) : base(param)
        {
        }

        public override void RunEffect()
        {
            base.RunEffect();
            Target.CharStat.Silenced = true;
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharStat.Silenced = false;
        }

        public override void Update()
        {
            Target.CharStat.Silenced = true;
        }
    }
    
    public class EffectTaunt : EffectBase
    {
        public EffectTaunt(EffectParameter param) : base(param){}

        public override void RunEffect()
        {
            Target.CharAI.TargetForcedFix(Caster);
        }
        
        public override void Update()
        {
            Target.CharAI.Taunt(Caster);
        }
        
        public override void EndEffect()
        {
            
        }

        
    }

    
}