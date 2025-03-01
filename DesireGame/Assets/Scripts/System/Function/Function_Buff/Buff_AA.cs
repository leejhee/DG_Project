using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// BUFF_AA : ���� {1}���� AA�� {2}��ŭ �߰����ظ� ���մϴ�.
    /// </summary>
    public class Buff_AA : BuffBase
    {
        private int _count = -1;
        private int _buffAmount = 0;

        public Buff_AA(BuffParameter buffParam) : base(buffParam)
        {
            _count = (int)_FunctionData.input1;
            _buffAmount = (int)_FunctionData.input2;
            
            // ������� ���� ������ ī��Ʈ ����, 0�� �� �� Function �Ҹ�
            _CastChar.CharStat.OnDealDamage += () =>
            {
                _count--;
                if (_count == 0)
                {
                    RunFunction(false);
                }
            };
        }

        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                OnStatBuff aaBuff = new()
                {
                    buffStat = SystemEnum.eStats.BONUS_DAMAGE,
                    opCode = SystemEnum.eOperator.Add,
                    amount = _buffAmount
                };

                OnKillBuff += () =>
                {
                    _CastChar.CharStat.RemoveBuff(aaBuff);
                    aaBuff.Dispose();                   
                };

                _CastChar.CharStat.PushBuffCalculator(aaBuff);
                
            }
            else
            {
                
            }
        }
    }
}