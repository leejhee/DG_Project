using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Client
{
    public class Inventory : Singleton<Inventory>
    {
        private readonly int maxCount = 100;
        private List<Item> itemList = new();
        public List<Item> ItemList => itemList;

        public Action<ItemUIParameter> OnItemChange; // 인벤토리에 변화 생길 시 UI에 변경사항 전달

        public override void Init()
        {
            base.Init();
            InitInventory();
        }

        // TODO : 저장 구조 확립 후 Load & Save 기능 구현하기
        private void InitInventory()
        {
            // TODO : 보유 아이템 불러오기
        }
        
        private void SaveInventory()
        {
            // TODO : 보유 아이템 목록 저장
        }

        public bool IsInventoryFull()
        {
            if (itemList.Count >= maxCount)
            {
                Debug.LogError($"인벤토리가 가득 차 아이템을 추가할 수 없습니다.");
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddItem(Item item)
        {
            if (IsInventoryFull()) return;

            itemList.Add(item);
            Debug.Log($"{item.ItemData.nameStringCode} (ID : {item.GetID()})  획득");

            OnItemChange?.Invoke(new ItemUIParameter(item));
        }

        public void DeleteItem(Item item)
        {
            if (!itemList.Contains(item))
            {
                Debug.LogError("해당 아이템은 인벤토리에 존재하지 않습니다.");
                return;
            }

            itemList.Remove(item);
            Debug.Log($"{item.ItemData.nameStringCode} (ID : {item.GetID()})  제거");

            OnItemChange?.Invoke(new ItemUIParameter(item));

        }
    }
}