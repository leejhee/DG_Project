using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// BUFF_AA : 다음 {1}번의 AA가 {2}만큼 추가피해를 가합니다.
    /// </summary>
    public class Buff_AA : BuffBase
    {
        private int _count = -1;
        private int _buffAmount = 0;

        public Buff_AA(BuffParameter buffParam) : base(buffParam)
        {
            _count = (int)_FunctionData.input1;
            _buffAmount = (int)_FunctionData.input2;
            
            // 대미지를 넣을 때마다 카운트 감소, 0이 될 시 Function 소멸
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