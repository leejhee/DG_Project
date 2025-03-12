using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class DamageByCasterAP : FunctionBase
    {
        public DamageByCasterAP(BuffParameter buffParam) : base(buffParam)
        {

        }
        public override void RunFunction(bool StartFunction)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                var stat = _CastChar.CharStat;
                _TargetChar.CharStat.ReceiveDamage(
                    stat.SendDamage(stat.GetStat(eStats.NAP) * (_FunctionData.input1 / SystemConst.PER_TEN_THOUSAND), eDamageType.MAGIC));
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            Debug.Log("�ð���ŭ ������. �� lifetime == 0�̸� 1�� ������");
        }

    }
}