using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public static class FunctionFactory
    {
        public static FunctionBase FunctionGenerate(BuffParameter buffParam)
        {
            switch (buffParam.eFunctionType)
            {
                case eFunction.None:                    return new NO_CONTENT(buffParam); 
                case eFunction.DAMAGE_BY_AD:            return new DamageByCasterAD(buffParam);
                case eFunction.DAMAGE_BY_AP:            return new DamageByCasterAP(buffParam);
                case eFunction.DAMAGE_BY_TARGET_MAXHP:  return new DamageByTargetMaxHP(buffParam);
                case eFunction.DOT_BY_AP:               return new DamageOverTimeByAP(buffParam);
                case eFunction.SYNERGY_TRIGGER:         return new SynergyTrigger(buffParam);
                case eFunction.BUFF_AA:                 return new Buff_AA(buffParam);
                case eFunction.ADDMANA_ON_AA:           return new AddMana_AA(buffParam);
                case eFunction.BUFF_ONCE:               return new BuffOnce(buffParam);
                case eFunction.CREATE_SHIELD:           return new CreateShield(buffParam);
                case eFunction.EXTEND_RANGE:            return new ExtendRange(buffParam);
                case eFunction.MULTICASTING:            return new MultiCasting(buffParam);
                case eFunction.APPLY_CC:                return new ApplyCC(buffParam);
                case eFunction.TELEPORT_TO_ALLY_REAR:   return new TeleportToAllyRear(buffParam);
                case eFunction.SWORD_SYNERGY_AABUFF:    return new SWORD_AA(buffParam);
                case eFunction.CHECK_CONDITION:         return new ConditionCheck(buffParam);
                default:
                    break;
            }
            return null;
        }

    }
}