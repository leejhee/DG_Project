using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Progress;

namespace Client
{
    public class ItemSlot
    {
        private CharPlayer charOwner;
        private List<Item> items = new(); // ������ ������ ����Ʈ


        private readonly int maxCount = 3;

        public void EquipItem(Item item)
        {
            // ������ ����
            if (items.Count >= maxCount)
            {
                Debug.Log("ĭ �� á�µ���");
                return;
            }

            items.Add(item);
            ApplyItemEffect(item);
        }

        public void UnequipItem(Item item)
        {
            // ������ ����
            if (items.Count >= maxCount)
            {
                Debug.Log("�� �� ���µ���");
                return;
            }

            items.Remove(item);
            DisapplyItemEffect(item);
        }
        public void ApplyItemEffect(Item item)
        {
            charOwner.CharStat.ChangeStateByBuff(item.ItemData.mainStats, item.ItemData.mainStatsIncrease);
            foreach(var substat in item.SubStatList)
            {
                charOwner.CharStat.ChangeStateByBuff(substat.eStat, substat.increase);
            }

        }
        public void DisapplyItemEffect(Item item)
        {
            charOwner.CharStat.ChangeStateByBuff(item.ItemData.mainStats, item.ItemData.mainStatsIncrease);
            foreach (var substat in item.SubStatList)
            {
                charOwner.CharStat.ChangeStateByBuff(substat.eStat, -substat.increase);
            }

        }

    }
}