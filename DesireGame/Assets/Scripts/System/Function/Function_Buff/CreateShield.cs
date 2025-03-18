using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// SHIELD�� �������� {statsType}�� {1}%��ŭ �� �� �����Ѵ�. Shield�� {time}ms ���� �������.
    /// </summary>
    /// <remarks> ��ȹ �ǵ��� ���� ������ ��ü�� ����ϵ��� ��.</remarks>
    public class CreateShield : FunctionBase
    {
        private Shield ShieldInstance = null;

        public CreateShield(BuffParameter buffParam) : base(buffParam)
        {
           
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if(StartFunction)
            {
                var stat = _CastChar.CharStat;
                if (stat == null) return;

                var shieldAmount = (long)(stat.GetStat(_FunctionData.statsType) *
                (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND));
                ShieldInstance = new Shield(shieldAmount, this);

                _TargetChar.CharStat.AddShield(ShieldInstance);
                Debug.Log($"�ǵ� �߰�: {_CastChar.GetID()}���� {_TargetChar.GetID()}������ �ǵ� �ο�");
            }
            else
            {
                _TargetChar.CharStat.RemoveShield(ShieldInstance);
                Debug.Log($"�ǵ� ����: {_CastChar.GetID()}���� {_TargetChar.GetID()}������ �� �ǵ� ����");
            }
        }

        public void OnShieldBreak()
        {
            _TargetChar.FunctionInfo.KillFunction(this);
        }

    }

    public class Shield
    {
        public long Amount { get; private set; }
        private CreateShield owner = null; 

        public Shield(long Amount, CreateShield owner)
        {
            this.Amount = Amount;
            this.owner = owner;
        }

        /// <returns> �� �ǵ� �ν��Ͻ����� ����ϰ� ���� ������� ��ȯ�Ѵ�. </returns>
        public long AbsorbDamage(long damage)
        {
            if (Amount >= damage)
            {
                Amount -= damage;
                return 0;
            }
            else
            {
                long remainingDamage = damage - Amount;
                Amount = 0;
                owner.OnShieldBreak();
                Debug.Log($"�ǵ� �μ���. ���� ����� : {remainingDamage}");
                return remainingDamage;
            }
        }

    }

}