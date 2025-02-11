using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// char ������Ʈ
    /// </summary>
    public class CharStat
    {
        private long[] _charStat = new long[(int)eStats.EMax];
        
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

        public void DamageHealth(int amount)
        {
            _charStat[(int)eStats.NHP] -= amount;
            if(_charStat[(int)eStats.NHP] < 0)
            {
                // ���⼭ ��� ó�� �ϱ�
            }
        }

    }
}