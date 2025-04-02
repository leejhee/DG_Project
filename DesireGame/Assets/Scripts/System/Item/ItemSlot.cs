using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Progress;

namespace Client
{
    public class ItemSlot
    {
        private CharPlayer charOwner;
        private List<Item> items = new(); // 장착한 아이템 리스트


        private readonly int maxCount = 3;

        public void EquipItem(Item item)
        {
            // 아이템 장착
            if (items.Count >= maxCount)
            {
                Debug.Log("칸 다 찼는데용");
                return;
            }

            items.Add(item);
            ApplyItemEffect(item);
        }

        public void UnequipItem(Item item)
        {
            // 아이템 해제
            if (items.Count >= maxCount)
            {
                Debug.Log("뺄 게 없는데용");
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