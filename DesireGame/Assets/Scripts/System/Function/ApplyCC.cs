using UnityEngine;

namespace Client
{
    public class ApplyCC : FunctionBase
    {
        private CCBase CC = null;

        public ApplyCC(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                CC = CCFactory.CCGenerate(new CCParameter()
                {
                    Caster = _CastChar,
                    Target = _TargetChar,
                    ccType = _FunctionData.CCType
                });
                CC?.RunCC();
            }
            else
            {
                CC?.EndCC();
            }

        }

    }

    public struct CCParameter
    {
        public CharBase Caster;
        public CharBase Target;
        public SystemEnum.eCCType ccType;
    }


    public static class CCFactory
    {
        public static CCBase CCGenerate(CCParameter param)
        {
            switch (param.ccType)
            {
                case SystemEnum.eCCType.LAYOFF:     return new CCLayOff(param);
                case SystemEnum.eCCType.STUN:       return new CCStun(param);
                case SystemEnum.eCCType.KNOCKBACK:  return new CCKnockBack(param);
            }
            return null;
        }
    }

    // [TODO] : CC별로 저장해야 할 이펙트 종류도 포함하는 게 좋을듯. 어떻게 담을지 고민해야한다.
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

    public class CCLayOff : CCBase
    {
        float delta = 0;

        public CCLayOff(CCParameter param) : base(param)
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


}
