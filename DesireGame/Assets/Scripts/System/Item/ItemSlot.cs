using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Progress;

namespace Client
{
    public class ItemSlot
    {
        private CharPlayer charOwner;
        private List<Item> items = new(); // ÀåÂøÇÑ ¾ÆÀÌÅÛ ¸®½ºÆ®


        private readonly int maxCount = 3;

        public void EquipItem(Item item)
        {
            // ¾ÆÀÌÅÛ ÀåÂø
            if (items.Count >= maxCount)
            {
                Debug.Log("Ä­ ´Ù Ã¡´Âµ¥¿ë");
                return;
            }

            items.Add(item);
            ApplyItemEffect(item);
        }

        public void UnequipItem(Item item)
        {
            // ¾ÆÀÌÅÛ ÇØÁ¦
            if (items.Count >= maxCount)
            {
                Debug.Log("»¬ °Ô ¾ø´Âµ¥¿ë");
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