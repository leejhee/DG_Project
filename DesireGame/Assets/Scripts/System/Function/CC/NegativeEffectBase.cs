using UnityEngine;

namespace Client
{
    public abstract class NegativeEffectBase
    {
        protected CharBase _Caster;
        protected CharBase _Target;
        public NegativeEffectBase(CCParameter param)
        {
            _Caster = param.Caster;
            _Target = param.Target;
        }

        public virtual void RunEffect()
        {
            Debug.Log($"{_Target.GetID()}번 캐릭터에 {GetType()} 타입의 CC 발동");
        }

        public virtual void EndEffect()
        {
            Debug.Log($"{_Target.GetID()}번 캐릭터에서 {GetType()} 타입의 CC 소멸");
        }
    }



    public abstract class DebuffBase : NegativeEffectBase
    {
        protected DebuffBase(CCParameter param) : base(param)
        {
        }
    }

    public abstract class CCBase : NegativeEffectBase
    {
        protected CCBase(CCParameter param) : base(param)
        {
        }
    }
}