using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;
using System;

namespace Client
{
    //[TODO] : ����� å�Ӻи� �� �� ���� �� �۾��� ��

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

            _charStat[(int)eStats.MOVE_SPEED] = (int)(charStat.moveSpeed * SystemConst.PER_THOUSAND);    // �̵� �ӵ�
            _charStat[(int)eStats.NMOVE_SPEED] = (int)(charStat.moveSpeed * SystemConst.PER_THOUSAND);   // ���� �̵� �ӵ�

            _charStat[(int)eStats.START_MANA] = charStat.startMana; // ����
            _charStat[(int)eStats.N_MANA] = charStat.startMana;     // ���� ����
            _charStat[(int)eStats.MAX_MANA] = charStat.maxMana;     // ���� �ִ� ����


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

        // UI�� ����ϸ� �ɵ�...?
        public Action OnDamaged;
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

        #endregion
    }
}