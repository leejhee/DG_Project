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
        public float amount;
        private CreateShield owner; 

        public Shield(float amount, CreateShield owner)
        {
            this.amount = amount;
            this.owner = owner;
        }

        /// <returns> �� �ǵ� �ν��Ͻ����� ����ϰ� ���� ������� ��ȯ�Ѵ�. </returns>
        public float AbsorbDamage(float damage)
        {
            if (amount >= damage)
            {
                amount -= damage;
                return 0f;
            }
            else
            {
                float remainingDamage = damage - amount;
                amount = 0f;
                owner.OnShieldBreak();
                Debug.Log($"�ǵ� �μ���. ���� ����� : {remainingDamage}");
                return remainingDamage;
            }
        }

    }

}