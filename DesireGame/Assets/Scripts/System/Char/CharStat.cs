using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// char 스테이트
    /// </summary>
    public class CharStat
    {
        private long[] _charStat = new long[(int)eStats.EMax];
        
        public CharStat(StatsData charStat)
        {
            _charStat[(int)eStats.AD] = charStat.AD;    // 공격력
            _charStat[(int)eStats.NAD] = charStat.AD;   // 현재 공격력

            _charStat[(int)eStats.AP] = charStat.AP;    // 주문력
            _charStat[(int)eStats.NAP] = charStat.AP;   // 현재 주문력

            _charStat[(int)eStats.HP] = charStat.HP;    // 체력
            _charStat[(int)eStats.NHP] = charStat.HP;   // 현재 체력
            _charStat[(int)eStats.NMHP] = charStat.HP;  // 현재 최대 체력

            _charStat[(int)eStats.AS] = charStat.attackSpeed;   // 공격 속도(천분율)
            _charStat[(int)eStats.NAS] = charStat.attackSpeed;  // 현재 공격 속도(천분율)

            _charStat[(int)eStats.CRIT_CHANCE] = charStat.critChance;       // 치명타 확률(만분율)
            _charStat[(int)eStats.NCRIT_CHANCE] = charStat.critChance;      // 현재 치명타 확률(만분율)

            _charStat[(int)eStats.CRIT_DAMAGE] = charStat.cirtDamage;       // 치명타 대미지(만분율)
            _charStat[(int)eStats.NCRIT_DAMAGE] = charStat.cirtDamage;      // 현재 치명타 대미지(만분율)

            _charStat[(int)eStats.DAMAGE_INCREASE] = charStat.damageIncrease;   // 피해량 증가(만분율)
            _charStat[(int)eStats.BONUS_DAMAGE] = charStat.bonusDamage;         // 추가 피해

            _charStat[(int)eStats.ARMOR] = charStat.defense;    // 방어력
            _charStat[(int)eStats.NARMOR] = charStat.defense;   // 현재 방어력

            _charStat[(int)eStats.MAGIC_RESIST] = charStat.magicResist;     // 마법 방어력
            _charStat[(int)eStats.NMAGIC_RESIST] = charStat.magicResist;    // 현재 마법 방어력

            _charStat[(int)eStats.ARMOR_PENETRATION] = 0;   // 물리 방어력 관통
            _charStat[(int)eStats.MAGIC_PENETRATION] = 0;   // 마법 방어력 관통

            _charStat[(int)eStats.RANGE] = charStat.Range;  // 공격 사거리
            _charStat[(int)eStats.NRANGE] = charStat.Range; // 현재 공격 사거리

            _charStat[(int)eStats.MOVE_SPEED] = (int)(charStat.moveSpeed * SystemConst.PER_THOUSAND);    // 이동 속도
            _charStat[(int)eStats.NMOVE_SPEED] = (int)(charStat.moveSpeed * SystemConst.PER_THOUSAND);   // 현재 이동 속도

            _charStat[(int)eStats.START_MANA] = charStat.startMana; // 마나
            _charStat[(int)eStats.N_MANA] = charStat.startMana;     // 현재 마나
            _charStat[(int)eStats.MAX_MANA] = charStat.maxMana;     // 현재 최대 마나


        }

        public float GetStat(eStats eState)
        {
            switch (eState)
            {
                case (eStats.AS):
                case (eStats.NAS):
                case (eStats.MOVE_SPEED):
                case (eStats.NMOVE_SPEED):
                    return _charStat[(int)eState] / SystemConst.PER_THOUSAND;
                case (eStats.CRIT_CHANCE):
                case (eStats.NCRIT_CHANCE):
                case (eStats.CRIT_DAMAGE):
                case (eStats.NCRIT_DAMAGE):
                case (eStats.DAMAGE_INCREASE):
                    return _charStat[(int)eState] / SystemConst.PER_TEN_THOUSAND;
                default:
                    return _charStat[(int)eState];
            }
        }

        public long GetStatRaw(eStats eState)
        {
            return _charStat[(int)eState];
        }

        public void DamageHealth(int amount)
        {
            _charStat[(int)eStats.NHP] -= amount;
            if(_charStat[(int)eStats.NHP] < 0)
            {
                // 여기서 사망 처리 하기
            }
        }

    }
}