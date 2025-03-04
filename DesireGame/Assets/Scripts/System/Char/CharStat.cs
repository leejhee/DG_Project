using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;
using System;
using UnityEditor.U2D.Animation;
using Unity.VisualScripting;

namespace Client
{
    public struct DamageParameter
    {
        public float rawDamage;        // 공격자 측에서 계산된 대미지
        public eDamageType damageType; // 공격자 측의 대미지 타입
        public float penetration;      // 대미지 타입에 따른 관통
    }

    /// <summary>
    /// char 스테이트
    /// </summary>
    public class CharStat
    {
        private CharBase StatOwner;

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

        public CharStat(StatsData charStat, CharBase StatOwner)
        {
            this.StatOwner = StatOwner;

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

            _charStat[(int)eStats.MOVE_SPEED] = (int)(charStat.moveSpeed * SystemConst.PER_THOUSAND);    // 이동 속도(천분율 처리함)
            _charStat[(int)eStats.NMOVE_SPEED] = (int)(charStat.moveSpeed * SystemConst.PER_THOUSAND);   // 현재 이동 속도(천분율 처리함)

            _charStat[(int)eStats.START_MANA] = charStat.startMana; // 마나
            _charStat[(int)eStats.N_MANA] = charStat.startMana;     // 현재 마나
            _charStat[(int)eStats.MAX_MANA] = charStat.maxMana;     // 현재 최대 마나

            _charStat[(int)eStats.MANA_RESTORE_INCREASE] = 0;       // 마나 회복량 추가 퍼센트(만분율)
            _charStat[(int)eStats.EFFECTIVE_HEALTH] = 0;            // 내구력 [TODO] : 정확히 알아볼 것
        }

        /// <summary> </summary>
        /// [TODO] : N자 들어가는거에 버프치 계산하는 구조 넣어서 할 것.
        public float GetStat(eStats eStats)
        {
            switch (eStats)
            {
                case (eStats.AS):
                case (eStats.NAS):
                case (eStats.MOVE_SPEED):
                case (eStats.NMOVE_SPEED):
                    return _charStat[(int)eStats] / SystemConst.PER_THOUSAND;
                case (eStats.CRIT_CHANCE):
                case (eStats.NCRIT_CHANCE):
                case (eStats.CRIT_DAMAGE):
                case (eStats.NCRIT_DAMAGE):
                case (eStats.DAMAGE_INCREASE):
                    return _charStat[(int)eStats] / SystemConst.PER_TEN_THOUSAND;
                default:
                    return _charStat[(int)eStats];
            }
        }

        public long GetStatRaw(eStats eState)
        {
            return _charStat[(int)eState];
        }

        public void ChangeStateByBuff(eStats stat, long delta)
        {
            eStats properTargetStat = CurrentStatByBaseStat(stat);         
            var afterStat = GetStatRaw(properTargetStat) + delta;
            _charStat[(int)properTargetStat] = afterStat;
        }

        // 스탯의 초기값이 같은 곳에 있기 때문에 이렇게 한다.
        public eStats CurrentStatByBaseStat(eStats baseStat)
        {
            switch (baseStat)
            {
                case eStats.AD:             return eStats.NAD; 
                case eStats.AP:             return eStats.NAP; 
                case eStats.HP:             return eStats.NHP;
                case eStats.AS:             return eStats.NAS;
                case eStats.CRIT_CHANCE:    return eStats.NCRIT_CHANCE;
                case eStats.CRIT_DAMAGE:    return eStats.NCRIT_CHANCE;
                case eStats.ARMOR:          return eStats.NARMOR;
                case eStats.MAGIC_RESIST:   return eStats.MAGIC_RESIST;
                case eStats.RANGE:          return eStats.NRANGE;
                case eStats.MOVE_SPEED:     return eStats.NMOVE_SPEED;
                default:
                    return baseStat;
            }
        }


        #region Damage Method

        // UI에 사용하면 될듯...?
        public Action OnDamaged;
        public Action OnDealDamage;
        public Action OnDeath;

        // 단순 '평타 강화' 등에 사용. AA 강화는 eStats로 써지지 않기 때문에.
        //private float[] TemporaryAdditionalDamage = new float[(int)eDamageType.eMax];
        //
        //public void InjectAdditionalDamage(eDamageType type, float delta)
        //{
        //    TemporaryAdditionalDamage[(int)type] += delta;
        //}



        /// <summary>
        /// 공격자 기준 대미지 관여 요소들을 산출합니다.
        /// </summary>
        /// <param name="statMultiplied"> 대미지에 반영할 스탯 * 반영 계수 </param>
        /// <param name="type"> None인 경우는 디폴트이므로, 평타인 경우입니다. 그 외는 스킬을 사용하면서 세팅합니다. </param>
        public DamageParameter SendDamage(float statMultiplied, eDamageType type = eDamageType.None)
        {
            float rawDamage =
                statMultiplied *                                        // 주스탯 * 계수
                (1 + GetStat(eStats.DAMAGE_INCREASE)) *                 // 피해량 증가
                (UnityEngine.Random.Range(0, 1) > GetStat(eStats.NCRIT_CHANCE) ?
                    (1 + GetStat(eStats.NCRIT_DAMAGE)) : 1)             //치명타 확률 및 피해 계산
                + GetStat(eStats.BONUS_DAMAGE);                         // 추가 대미지  

            // 평타면 여기서 평타 강화에 쓰일 것이고, 스킬 뎀지 강화면 여기서 별도의 추뎀이 더해질 것이다.
            //pureDamage += TemporaryAdditionalDamage[(int)type];

            eDamageType damageType = type == eDamageType.None ? DamageType : type;

            float penetration = DamageType == eDamageType.PHYSICS ? 
                GetStat(eStats.ARMOR_PENETRATION) : GetStat(eStats.MAGIC_PENETRATION);

            

            OnDealDamage?.Invoke();

            return new DamageParameter()
            {
                rawDamage = rawDamage,
                damageType = damageType,
                penetration = penetration
            };
        }

        // 내구력, 방어력 참고하여 쓸 것.
        // 실드에의 대미지는 다 다른가요??

        public void ReceiveDamage(DamageParameter damage)
        {
            // 최종대미지 계산 파트
            float finalDamage = 0;
            if(damage.damageType == eDamageType.TRUE)
            {
                finalDamage = damage.rawDamage;
            }
            else
            {
                float defender = damage.damageType == eDamageType.PHYSICS ?
                    GetStat(eStats.NARMOR) : GetStat(eStats.NMAGIC_RESIST);

                finalDamage =
                    damage.rawDamage *
                    100f / (100 + defender - damage.penetration);
            }

            var appliedDamage = (long)finalDamage;

            // 실드 계산 파트
            if(_charStat[(int)eStats.SHIELD] > 0)
            {
                if (_charStat[(int)eStats.SHIELD] > appliedDamage)
                {
                    _charStat[(int)eStats.SHIELD] -= appliedDamage;
                    Debug.Log($"보호막에 대미지 {appliedDamage}만큼 받음. 잔여 보호막 {GetStat(eStats.SHIELD)}");
                }
            }            
            else
            {                
                appliedDamage -= _charStat[(int)eStats.SHIELD];
                _charStat[(int)eStats.SHIELD] = 0;
            }

            // 실대미지 계산 파트
            _charStat[(int)eStats.NHP] -= appliedDamage;
            if(StatOwner.Index != 100)
            {
                Debug.Log($"대미지 {appliedDamage}만큼 받음. 잔여 HP {GetStat(eStats.NHP)}");
            }
            OnDamaged?.Invoke();

            // 사망 검사 파트
            if (_charStat[(int)eStats.NHP] < 0)
            {
                Debug.Log("으엑 죽었다");
                OnDeath?.Invoke();
            }
        }


        public void GainMana(int amount, bool isGain)
        {
            var increaseRatio = GetStat(eStats.MANA_RESTORE_INCREASE);
            if (isGain)
            {
                _charStat[(int)eStats.N_MANA] += (int)((1 + increaseRatio) * amount);
            }
            else
            {
                _charStat[(int)eStats.N_MANA] -= amount;
            }
        }


        #endregion
        
    }

}