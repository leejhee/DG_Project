using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;
using System;

namespace Client
{
    public struct DamageParameter
    {
        public float pureDamage;       // ������ ������ ���� �����
        public eDamageType damageType; // ������ ���� ����� Ÿ��
        public float penetration;      // ����� Ÿ�Կ� ���� ����
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
            } }

        public CharStat(StatsData charStat, CharBase StatOwner)
        {
            this.StatOwner = StatOwner;

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
            _charStat[(int)eStats.EFFECTIVE_HEALTH] = 0;            // ������ [TODO] : ��Ȯ�� �˾ƺ� ��
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

        // UI�� ����ϸ� �ɵ�...?
        public Action OnDamaged;
        public Action OnDealDamage;
        public Action OnDeath;

        /// <summary>
        /// ������ ���� ����� ���� ��ҵ��� �����մϴ�.
        /// </summary>
        /// <param name="statMultiplied"> ������� �ݿ��� ���� * �ݿ� ��� </param>
        /// <param name="type"> None�� ���� ����Ʈ�̹Ƿ�, ��Ÿ�� ����Դϴ�. �� �ܴ� ��ų�� ����ϸ鼭 �����մϴ�. </param>
        public DamageParameter SendDamage(float statMultiplied, eDamageType type = eDamageType.None)
        {
            float pureDamage =
                statMultiplied *                                        // �ֽ��� * ���
                (1 + GetStat(eStats.DAMAGE_INCREASE)) *                 // ���ط� ����
                (UnityEngine.Random.Range(0, 1) > GetStat(eStats.NCRIT_CHANCE) ?
                    (1 + GetStat(eStats.NCRIT_DAMAGE)) : 1)             //ġ��Ÿ Ȯ�� �� ���� ���
                + GetStat(eStats.BONUS_DAMAGE);                         // �߰� �����  

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

        // ������, ���� �����Ͽ� �� ��.
        public void ReceiveDamage(DamageParameter damage)
        {
            float defender = damage.damageType == eDamageType.PHYSICS ?
                GetStat(eStats.NARMOR) : GetStat(eStats.NMAGIC_RESIST);

            float finalDamage = 
                damage.pureDamage * 
                100f / (100 + defender - damage.penetration);

            _charStat[(int)eStats.NHP] -= (long)finalDamage;
            Debug.Log($"����� {(long)finalDamage}��ŭ ����. �ܿ� HP {GetStat(eStats.NHP)}");
            OnDamaged?.Invoke();

            if (_charStat[(int)eStats.NHP] < 0)
            {
                Debug.Log("���� �׾���");
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


        /// <summary> ���� ���� �߻� �� ���� �� �ش� ���ȿ� ���� ������Ʈ �ǽ� </summary>
        /// <remarks> ��Ÿ�� �� <b>'���� ����'</b> �� ���ؼ��� ����� ��</remarks>
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

    /// <summary> �ϴ� ��ü�� ��������� ������ </summary>
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