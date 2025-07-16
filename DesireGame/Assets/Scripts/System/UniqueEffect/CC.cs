using UnityEngine;

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
            // 코루틴 쓰자.
            // 넉백하는동안, 인수만 집어서 코루틴 작동하게 한다. 이때 원래 AI clear하고 정지.
        }

        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharAI.RestoreState();
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
            Target.CharAI.Stun();
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharAI.RestoreState();
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
            Target.CharAI.Charm(Caster);
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharAI.RestoreState();
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
            base.RunEffect();
            Target.CharAI.Taunt(Caster);
        }
        
        public override void EndEffect()
        {
            base.EndEffect();
            Target.CharAI.RestoreState();
        }

        
    }

    
}