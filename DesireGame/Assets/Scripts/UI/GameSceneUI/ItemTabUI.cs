using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class ItemTabUI : MonoBehaviour
    {
        [SerializeField] Transform itemGridPanel; // Vertical Layout Panel의 Transform
        [SerializeField] GameObject itemSlotPrefab;
        [SerializeField] Button NextPageButton;
        [SerializeField] ItemInfoPanelUI itemInfoPanel;

        private List<ItemSlotUI> itemSlots = new();
        private int currentPage = 0;
        private const int maxCountPerPage = 9;

        private void Start()
        {
            Inventory.Instance.OnItemChange += RefreshItemUI;

            NextPageButton.onClick.AddListener(GoNextPage);
        }

        private void InitSlots()
        {
            for (int i = 0; i < maxCountPerPage; i++)
            {
                GameObject slotObj = Instantiate(itemSlotPrefab, itemGridPanel);
                ItemSlotUI slot = slotObj.GetComponent<ItemSlotUI>();
                slot.Setup(OnItemSlotClicked);
                itemSlots.Add(slot);
            }
        }

        private void OnItemSlotClicked(Item item)
        {
            itemInfoPanel.Show(item); // 아이템 정보 표시
        }

        private void RefreshItemUI(ItemUIParameter itemParameter)
        {
            List<Item> items = Inventory.Instance.ItemList;

            int totalPages = Mathf.CeilToInt((float)items.Count / maxCountPerPage);
            currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(totalPages - 1, 0));
            
            int startIndex = currentPage * maxCountPerPage;

            for (int i = 0; i < maxCountPerPage; i++)
            {
                if (startIndex + i < items.Count)
                {
                    itemSlots[i].SetItem(items[startIndex + i]);
                }
                else
                {
                    itemSlots[i].Clear();
                }
            }
        }


        private void GoNextPage()
        {
            int totalPages = Mathf.CeilToInt((float)Inventory.Instance.ItemList.Count / maxCountPerPage);

            currentPage = (currentPage + 1) % Mathf.Max(totalPages, 1);
            RefreshItemUI(null);
        }

    }
}