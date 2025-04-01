using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;
using System;
using static Client.CharAI;

namespace Client
{
    public struct DamageParameter
    {
        public CharBase Attacker;       // ������ �Ż�
        public float rawDamage;         // ������ ������ ���� �����
        public eDamageType damageType;  // ������ ���� ����� Ÿ��
        public float penetration;       // ����� Ÿ�Կ� ���� ����
    }

    /// <summary>
    /// char ������Ʈ
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
            } 
        }

        public CharStat(StatsData charStat, CharBase StatOwner)
        {
            this.StatOwner = StatOwner;

            #region Init Stats

            _charStat[(int)eStats.AD] = charStat.AD;    // ���ݷ�
            _charStat[(int)eStats.NAD] = charStat.AD;   // ���� ���ݷ�

            _charStat[(int)eStats.AP] = charStat.AP;    // �ֹ���
            _charStat[(int)eStats.NAP] = charStat.AP;   // ���� �ֹ���

            _charStat[(int)eStats.HP] = charStat.HP;    // ü��
            _charStat[(int)eStats.NHP] = charStat.HP;   // ���� ü��
            _charStat[(int)eStats.NMHP] = charStat.HP;  // ���� �ִ� ü��

            _charStat[(int)eStats.AS] = charStat.attackSpeed;   // ���� �ӵ�(õ����)
            _charStat[(int)eStats.NAS] = charStat.attackSpeed;  // ���� ���� �ӵ�(õ����)

            _charStat[(int)eStats.CRIT_CHANCE] = charStat.critChance;       // ġ��Ÿ Ȯ��(������)
            _charStat[(int)eStats.NCRIT_CHANCE] = charStat.critChance;      // ���� ġ��Ÿ Ȯ��(������)

            _charStat[(int)eStats.CRIT_DAMAGE] = charStat.cirtDamage;       // ġ��Ÿ �����(������)
            _charStat[(int)eStats.NCRIT_DAMAGE] = charStat.cirtDamage;      // ���� ġ��Ÿ �����(������)

            _charStat[(int)eStats.DAMAGE_INCREASE] = charStat.damageIncrease;   // ���ط� ����(������)
            _charStat[(int)eStats.BONUS_DAMAGE] = charStat.bonusDamage;         // �߰� ����

            _charStat[(int)eStats.ARMOR] = charStat.defense;    // ����
            _charStat[(int)eStats.NARMOR] = charStat.defense;   // ���� ����

            _charStat[(int)eStats.MAGIC_RESIST] = charStat.magicResist;     // ���� ����
            _charStat[(int)eStats.NMAGIC_RESIST] = charStat.magicResist;    // ���� ���� ����

            _charStat[(int)eStats.ARMOR_PENETRATION] = 0;   // ���� ���� ����
            _charStat[(int)eStats.MAGIC_PENETRATION] = 0;   // ���� ���� ����

            _charStat[(int)eStats.RANGE] = charStat.Range;  // ���� ��Ÿ�
            _charStat[(int)eStats.NRANGE] = charStat.Range; // ���� ���� ��Ÿ�

            _charStat[(int)eStats.MOVE_SPEED] = (int)(charStat.moveSpeed * SystemConst.PER_THOUSAND);    // �̵� �ӵ�(õ���� ó����)
            _charStat[(int)eStats.NMOVE_SPEED] = (int)(charStat.moveSpeed * SystemConst.PER_THOUSAND);   // ���� �̵� �ӵ�(õ���� ó����)

            _charStat[(int)eStats.START_MANA] = charStat.startMana; // ����
            _charStat[(int)eStats.N_MANA] = charStat.startMana;     // ���� ����
            _charStat[(int)eStats.MAX_MANA] = charStat.maxMana;     // ���� �ִ� ����

            _charStat[(int)eStats.MANA_RESTORE_INCREASE] = 0;       // ���� ȸ���� �߰� �ۼ�Ʈ(������)
            _charStat[(int)eStats.DAMAGE_REDUCTION] = 0;            // ������ (���� ���ط� �ۼ�Ʈ �氨)

            #endregion

        }

        /// <summary> </summary>
        /// [TODO] : N�� ���°ſ� ����ġ ����ϴ� ���� �־ �� ��.
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
                case (eStats.DAMAGE_REDUCTION):
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
            var tempStat = GetStatRaw(properTargetStat);
            var afterStat = GetStatRaw(properTargetStat) + delta;

            switch (properTargetStat)
            {
                case eStats.NHP:
                    _charStat[(int)eStats.NHP] = (long)Mathf.Clamp(_charStat[(int)eStats.NHP], 0, _charStat[(int)eStats.NMHP]);
                    break;
                default:
                    _charStat[(int)properTargetStat] = afterStat;
                    break;
            }

            Debug.Log($"{StatOwner.GetID()}�� ĳ���Ϳ��� {properTargetStat} ���� {tempStat} -> {afterStat}");
        }

        // ������ �ʱⰪ�� ���� ���� �ֱ� ������ �̷��� �Ѵ�.
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
                case eStats.MAGIC_RESIST:   return eStats.NMAGIC_RESIST;
                case eStats.RANGE:          return eStats.NRANGE;
                case eStats.MOVE_SPEED:     return eStats.NMOVE_SPEED;
                default:
                    return baseStat;
            }
        }


        #region Damage Method

        // UI�� ����ϸ� �ɵ�...?
        public Action OnDamaged;
        public Action OnDealDamage;
        public Action OnDeath;

        public Action<CharBase> �Ż����;

        /// <summary>
        /// ������ ���� ����� ���� ��ҵ��� �����մϴ�.
        /// </summary>
        /// <param name="statMultiplied"> ������� �ݿ��� ���� * �ݿ� ��� </param>
        /// <param name="type"> None�� ���� ����Ʈ�̹Ƿ�, ��Ÿ�� ����Դϴ�. �� �ܴ� ��ų�� ����ϸ鼭 �����մϴ�. </param>
        public DamageParameter SendDamage(float statMultiplied, eDamageType type = eDamageType.None)
        {
            float rawDamage =
                (statMultiplied *                                           // �ֽ��� * ���                                  
                (UnityEngine.Random.Range(0, 1) > GetStat(eStats.NCRIT_CHANCE) ?
                    (1 + GetStat(eStats.NCRIT_DAMAGE)) : 1)                 // ġ��Ÿ Ȯ�� �� ���� ���
                + GetStat(eStats.BONUS_DAMAGE)) *                           // �߰� �����  
                (1 + GetStat(eStats.DAMAGE_INCREASE));                      // ���ط� ����                 
            
            eDamageType damageType = type == eDamageType.None ? DamageType : type;

            float penetration = GetPenetration(damageType);

            OnDealDamage?.Invoke();

            return new DamageParameter()
            {
                Attacker = StatOwner,
                rawDamage = rawDamage,
                damageType = damageType,
                penetration = penetration
            };
        }

        public float GetPenetration(eDamageType damageType)
        {
            return damageType == eDamageType.PHYSICS ?
                GetStat(eStats.ARMOR_PENETRATION) : GetStat(eStats.MAGIC_PENETRATION);
        }


        public void ReceiveDamage(DamageParameter damage)
        {
            if (_charStat[(int)eStats.NHP] <= 0) return;

            // ��������� ��� ��Ʈ(������ �ݿ�)
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
                    100f / (100 + defender - damage.penetration) *
                    (1 - GetStat(eStats.DAMAGE_REDUCTION));

            }

            // �ǵ� ��� ��Ʈ
            var appliedDamage = AbsorbDamage((long)finalDamage);

            // �Ǵ���� ��� ��Ʈ
            _charStat[(int)eStats.NHP] -= appliedDamage;           
             Debug.Log($"{StatOwner.GetID()}�� ���� ����� {appliedDamage}��ŭ ����. �ܿ� HP {GetStat(eStats.NHP)}");
            
            OnDamaged?.Invoke();

            // ��� �˻� ��Ʈ
            if (_charStat[(int)eStats.NHP] <= 0)
            {
                Debug.Log("���� �׾���");
                �Ż����?.Invoke(damage.Attacker);
                OnDeath?.Invoke();
            }
        }

        #endregion

        #region Shield Managing
        //[TODO] : ĸ��ȭ ���ص� �Ƿ���...? ������ �� �غ���.

        // Function�� ���� �ǵ带 ��´�.
        private readonly List<Shield> Shields = new();

        public void AddShield(Shield shield) 
        {
            Shields.Add(shield);
            _charStat[(int)eStats.SHIELD] += (long)shield.Amount;
        } 
        public void RemoveShield(Shield shield) => Shields.Remove(shield);
        public long AbsorbDamage(long damage)
        {
            long remainingDamage = damage;
            for (int i = Shields.Count - 1; i >= 0 && remainingDamage > 0; i--)
            {
                Shield shield = Shields[i];
                remainingDamage = shield.AbsorbDamage(remainingDamage);
            }

            return remainingDamage;
        }
        #endregion


        #region Mana Exchanging Function

        public void GainMana(eAttackMode mode)
        {
            if (mode == eAttackMode.Auto)
            {
                if (GetStat(eStats.MAX_MANA) != 0)
                    GainMana(SystemConst.DEFAULT_MANA_RESTORE, true);
            }
            else if (mode == eAttackMode.Skill)
            {
                GainMana((int)GetStat(eStats.MAX_MANA), false);
            }
        }

        public void GainMana(int amount, bool isGain, bool isAdditional=false)
        {
            var increaseRatio = GetStat(eStats.MANA_RESTORE_INCREASE);
            if (isGain && !isAdditional)
            {
                _charStat[(int)eStats.N_MANA] += (int)((1 + increaseRatio) * amount);
            }
            else if(isGain && isAdditional)
            {
                _charStat[(int)eStats.N_MANA] += amount;
            }
            else
            {
                _charStat[(int)eStats.N_MANA] -= amount;
            }
        }


        #endregion
        
        public void ResetAfterBattle()
        {
            // ���� ���� ü�°� ���� �ʱ�ȭ
            // �̰� �����Ѱ�?
            _charStat[(int)eStats.NHP] = (long)GetStat(eStats.NMHP);
            _charStat[(int)eStats.N_MANA] = (long)GetStat(eStats.START_MANA);
            Debug.Log($"���� : ü�� {GetStat(eStats.NHP)}/{GetStat(eStats.NMHP)} ���� {GetStat(eStats.N_MANA)}/{GetStat(eStats.START_MANA)}");
        }

    }
}