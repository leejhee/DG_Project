using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;
using System;

namespace Client
{
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


        #region Damage Method

        // UI에 사용하면 될듯...?
        public Action OnDamaged;
        public Action OnDealDamage;
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
        
        private Dictionary<eStats, StatCalculator> _buffCalculator = new();

        public void PushBuffCalculator(OnStatBuff buff)
        {
            if (!_buffCalculator.ContainsKey(buff.buffStat))
            {
                _buffCalculator.Add(buff.buffStat, new StatCalculator());
            }
            _buffCalculator[buff.buffStat].PushBuffOperand(buff);
            UpdateCurrentStat(buff.buffStat);
        }

        public void RemoveBuff(OnStatBuff buff)
        {
            if (_buffCalculator.ContainsKey(buff.buffStat))
            {
                _buffCalculator[buff.buffStat].KillBuffOperand(buff);
                UpdateCurrentStat(buff.buffStat);
            }
        }


        /// <summary> 스탯 버프 발생 및 제거 시 해당 스탯에 대한 업데이트 실시 </summary>
        /// <remarks> 런타임 중 <b>'현재 스탯'</b> 에 대해서만 사용할 것</remarks>
        public void UpdateCurrentStat(eStats targetStat)
        {
            _charStat[(int)targetStat] =
                (long)_buffCalculator[targetStat].GetBuffedResult(GetStatRaw(targetStat));
        }



    }

    public struct OnStatBuff : IDisposable
    {
        public eStats buffStat;
        public eOperator opCode;
        public int amount;

        public readonly void Dispose(){}
    }

    /// <summary> 일단 롤체식 버프계산기로 가져옴 </summary>
    public class StatCalculator
    {       
        private Dictionary<eOperator, List<OnStatBuff>> _operDict = new(); 
        
        public float GetBuffedResult(float input)
        {
            float result = input;
            if(_operDict.TryGetValue(eOperator.Add, out var addBuffs))
            {
                foreach(var buff in addBuffs)
                {
                    result += buff.amount;
                }
            }
            if(_operDict.TryGetValue(eOperator.Mul, out var mulBuffs))
            {
                float ratio = 0f;
                foreach(var buff in mulBuffs)
                {
                    ratio += buff.amount;
                }
                ratio /= SystemConst.PER_TEN_THOUSAND;
                result *= (1 + ratio);
            }
            return result;
        }


        public void PushBuffOperand(OnStatBuff buff)
        {
            if (!_operDict.ContainsKey(buff.opCode))
            {
                _operDict.Add(buff.opCode, new List<OnStatBuff>());
            }
            _operDict[buff.opCode].Add(buff);
        }

        public void KillBuffOperand(OnStatBuff buff)
        {
            _operDict[buff.opCode].Remove(buff);
        }
    }
}