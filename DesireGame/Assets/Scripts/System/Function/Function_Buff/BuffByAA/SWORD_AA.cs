using UnityEngine;

namespace Client
{
    public class SWORD_AA : FunctionBase
    {
        private int _count = -1;
        private int _amount = 0;
        
        public SWORD_AA(BuffParameter buffParam) : base(buffParam)
        {
            _count = (int)_FunctionData.input1;
            _amount = (int)_FunctionData.input2;
        }

        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _CastChar.CharAI.OnTargetSet += SetFunctionTarget;
                _CastChar.CharStat.OnDealDamage += OnAA;
            }
            else
            {
                _CastChar.CharAI.OnTargetSet -= SetFunctionTarget;
                _CastChar.CharStat.OnDealDamage -= OnAA;
            }
        }

        private void SetFunctionTarget(CharBase charBase) => _TargetChar = charBase;

        public void OnAA()
        {
            if(_TargetChar == null) return;

            if(_count == 0)
            {
                _TargetChar.CharStat.ReceiveDamage(new DamageParameter()
                {
                    rawDamage = _amount,
                    damageType = _FunctionData.damageType,
                    penetration = _TargetChar.CharStat.GetPenetration(_FunctionData.damageType)
                });
                Debug.Log($"{_CastChar.name}이 {_TargetChar.GetID()}번 유닛에게 시너지 효과로 대미지 추가시킴");
                _count = (int)_FunctionData.input1;
            }
            else 
            {
                _count--;
            }

        }
    }
}