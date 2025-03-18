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
                _CastChar.CharStat.OnDealDamage += OnAA;
            }
            else
            {
                _CastChar.CharStat.OnDealDamage -= OnAA;
            }
        }

        public void OnAA()
        {
            if(_count == 0)
            {
                _TargetChar.CharStat.ReceiveDamage(new DamageParameter()
                {
                    rawDamage = _amount,
                    damageType = _FunctionData.damageType,
                    penetration = _TargetChar.CharStat.GetPenetration(_FunctionData.damageType)
                });
                Debug.Log($"{_TargetChar.GetID()}번 유닛 대미지 추가로 받음");
                _count = (int)_FunctionData.input1;
            }
            else 
            {
                _count--;
            }

        }
    }
}