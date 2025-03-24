using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class Inventory : Singleton<Inventory>
    {
        // �̱��� �ʹ� �����ϳ�?
        // �ٵ� �̱����� ���� �ʰ��� ������ ���� ���� ��.��

        private readonly int maxCount = 20;
        private List<Item> itemList = new();
        public override void Init()
        {
            base.Init();
            InitInventory();
        }

        // TODO : ���� ���� Ȯ�� �� Load & Save ��� �����ϱ�
        private void InitInventory()
        {
            // TODO : ���� ������ �ҷ�����
        }
        
        private void SaveInventory()
        {
            // TODO : ���� ������ ��� ����
        }

        public bool IsInventoryFull()
        {
            if (itemList.Count >= maxCount)
            {
                Debug.LogError($"�κ��丮�� ���� �� �������� �߰��� �� �����ϴ�.");
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
            Debug.Log($"{item.ItemData.nameStringCode} (ID : {item.GetID()})  ȹ��");
        }

        public void DeleteItem(Item item)
        {
            if (!itemList.Contains(item))
            {
                Debug.LogError("�ش� �������� �κ��丮�� �������� �ʽ��ϴ�.");
                return;
            }

            itemList.Remove(item);
            Debug.Log($"{item.ItemData.nameStringCode} (ID : {item.GetID()})  ����");
        }

        // TODO : ������ ����Ʈ ����� UI ������Ʈ
    }
}