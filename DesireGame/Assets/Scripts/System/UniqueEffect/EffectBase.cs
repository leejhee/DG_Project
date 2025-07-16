using UnityEngine;

namespace Client
{
    public abstract class EffectBase
    {
        protected CharBase Caster;
        protected CharBase Target;
        protected float LifeTime;
        protected float StartTime;
        
        public SystemEnum.eCCType EffectType { get; }
        

        public EffectBase(EffectParameter param)
        {
            Caster = param.Caster;
            Target = param.Target;
            LifeTime = param.Time;
            EffectType = param.ccType;
        }
        
        public virtual void RunEffect()
        {
            Debug.Log($"{Target.GetID()}번 캐릭터에 {GetType()} 타입의 효과 발동");
        }

        public virtual void EndEffect()
        {
            Debug.Log($"{Target.GetID()}번 캐릭터에서 {GetType()} 타입의 효과 소멸");
        }

        public virtual void Update() { }

        public void CheckTimeOver()
        {
            if (LifeTime == -1f) return;
            float runTime = Time.time - StartTime;
            if (runTime > LifeTime || LifeTime == 0f)
            {
                Target.EffectInfo.KillEffect(this);
            }
        }
    }
}