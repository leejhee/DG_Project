using UnityEngine;

namespace Client
{
    public abstract class CCBase
    {
        protected CharBase _Caster;
        protected CharBase _Target;
        public CCBase(CCParameter param)
        {
            _Caster = param.Caster;
            _Target = param.Target;
        }

        public virtual void RunCC()
        {
            Debug.Log($"{_Target.GetID()}번 캐릭터에 {GetType()} 타입의 CC 발동");
        }

        public virtual void EndCC()
        {
            Debug.Log($"{_Target.GetID()}번 캐릭터에서 {GetType()} 타입의 CC 소멸");
        }
    }
}