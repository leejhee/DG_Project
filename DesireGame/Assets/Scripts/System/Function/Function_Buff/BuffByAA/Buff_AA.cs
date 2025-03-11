using UnityEngine;

namespace Client
{
    /// <summary>
    /// BUFF_AA : ���� {1}���� AA�� {2}��ŭ �߰����ظ� ���մϴ�.
    /// </summary>
    public class Buff_AA : FunctionBase
    {
        private int _count = -1;
        private int _buffAmount = 0;

        public Buff_AA(BuffParameter buffParam) : base(buffParam)
        {
            _count = (int)_FunctionData.input1;
            _buffAmount = (int)_FunctionData.input2;                
        }

        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _CastChar.CharStat.OnDealDamage += SendReinforcedDamage;
            }
            else
            {
                _CastChar.CharStat.OnDealDamage -= SendReinforcedDamage;
            }
        }

        public void SendReinforcedDamage()
        {
            if(_count > 0)
            {
                Debug.Log(@$"{_CastChar.GetID()}�� ��ų �ߵ����� ��Ÿ�� {_buffAmount}��ŭ ����.
count : {_count}");
                _count--;
                
                _TargetChar.CharStat.ReceiveDamage(new DamageParameter()
                {
                    rawDamage = _buffAmount,
                    damageType = SystemEnum.eDamageType.TRUE,
                    penetration = 0
                });
            }           
            else if (_count == 0)
            {
                _TargetChar.FunctionInfo.KillFunction(this);
            }
        }

    }
}