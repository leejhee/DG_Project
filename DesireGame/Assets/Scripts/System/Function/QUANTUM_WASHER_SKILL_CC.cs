using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class QUANTUM_WASHER_SKILL_CC_2 : FunctionBase
    {
        public QUANTUM_WASHER_SKILL_CC_2(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _CastChar.CharAction.OnAttackAction += ApplyQuantum2;
            }
            else
            {
                _CastChar.CharAction.OnAttackAction -= ApplyQuantum2;
            }
        }

        private void ApplyQuantum2(CharAI.eAttackMode mode, List<CharBase> SkillTargets)
        {
            if (mode == CharAI.eAttackMode.Skill)
            {
                foreach (var target in SkillTargets)
                {
                    if (target.GetCharType() != CharUtil.GetEnemyType(_CastChar.GetCharType()))
                        continue;
                    Debug.Log($"[시너지 효과] : 캐릭터 {_CastChar}의 공격에 의해 {_TargetChar}에 CC발생. ");
                    _TargetChar.EffectInfo.AddEffect(new EffectParameter()
                    {
                        Caster = _CastChar,
                        Target = _TargetChar,
                        ccType = SystemEnum.eCCType.SHRED,
                        Time = _FunctionData.input1
                    });
                }
                // 상대에게 CC를 넣자.
                
                
            }
        }
    }

    public class QUANTUM_WASHER_SKILL_CC_3 : FunctionBase
    {
        public QUANTUM_WASHER_SKILL_CC_3(BuffParameter buffParam) : base(buffParam)
        {
        }
        
        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _CastChar.CharAction.OnAttackAction += ApplyQuantum3;
            }
            else
            {
                _CastChar.CharAction.OnAttackAction -= ApplyQuantum3;
            }
        }

        private void ApplyQuantum3(CharAI.eAttackMode mode, List<CharBase> SkillTargets)
        {
            if (mode == CharAI.eAttackMode.Skill)
            {
                foreach (var target in SkillTargets)
                {
                    if (target.GetCharType() != CharUtil.GetEnemyType(_CastChar.GetCharType()))
                        continue;
                }
                // 상대에게 CC를 넣자.
                
                
            }
        }
        
    }
    
    
}