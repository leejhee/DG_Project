using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;
using System;

namespace Client
{
    //[TODO] : 대미지 책임분리 할 지 결정 후 작업할 것

    public struct DamageParameter
    {
        public float pureDamage;       // 공격자 측에서 계산된 대미지
        public eDamageType damageType; // 공격자 측의 대미지 타입
        public float penetration;      // 대미지 타입에 따른 관통
    }

    /// <summary>
    /// char 스테이트
    /// </summary>
    public class CharStat
    {
        private long[] _charStat = new long[(int)eStats.eMax];

        public eDamageType DamageType { get
            {
                if (_charStat[(int)eStats.AD] > 0 && _charStat[(int)eStats.AP] == 0)
                    return eDamageType.PHYSICS;
                else if (_charStat[(int)eStats.AP] > 0 && _charStat[(int)eStats.AD] == 0)
                    return eDamageType.MAGIC;
                else
                    return eDamageType.None;
            } }

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


        #region Damage Method

        // UI에 사용하면 될듯...?
        public Action OnDamaged;
        public Action OnDeath;

        /// <summary>
        /// 공격자 기준 대미지 관여 요소들을 산출합니다.
        /// </summary>
        /// <param name="statMultiplied"> 대미지에 반영할 스탯 * 반영 계수 </param>
        /// <param name="type"> None인 경우는 디폴트이므로, 평타인 경우입니다. 그 외는 스킬을 사용하면서 세팅합니다. </param>
        public DamageParameter SendDamage(float statMultiplied, eDamageType type = eDamageType.None)
        {
            float pureDamage =
                statMultiplied *                                        // 주스탯 * 계수
                (1 + GetStat(eStats.DAMAGE_INCREASE)) *                 // 피해량 증가
                (UnityEngine.Random.Range(0, 1) > GetStat(eStats.NCRIT_CHANCE) ?
                    (1 + GetStat(eStats.NCRIT_DAMAGE)) : 1)             //치명타 확률 및 피해 계산
                + GetStat(eStats.BONUS_DAMAGE);                         // 추가 대미지  

            eDamageType damageType = type == eDamageType.None ? DamageType : type;

            float penetration = DamageType == eDamageType.PHYSICS ? 
                GetStat(eStats.ARMOR_PENETRATION) : GetStat(eStats.MAGIC_PENETRATION);

            return new DamageParameter()
            {
                pureDamage = pureDamage,
                damageType = damageType,
                penetration = penetration
            };
        }

        // 내구력, 방어력 참고하여 쓸 것.
        public void ReceiveDamage(DamageParameter damage)
        {
            float defender = damage.damageType == eDamageType.PHYSICS ?
                GetStat(eStats.NARMOR) : GetStat(eStats.NMAGIC_RESIST);

            float finalDamage = 
                damage.pureDamage * 
                100f / (100 + defender - damage.penetration);

            _charStat[(int)eStats.NHP] -= (long)finalDamage;
            Debug.Log($"대미지 {(long)finalDamage}만큼 받음. 잔여 HP {GetStat(eStats.NHP)}");
            OnDamaged?.Invoke();

            if (_charStat[(int)eStats.NHP] < 0)
            {
                Debug.Log("으엑 죽었다");
                OnDeath?.Invoke();
            }
        }

        #endregion
    }
}