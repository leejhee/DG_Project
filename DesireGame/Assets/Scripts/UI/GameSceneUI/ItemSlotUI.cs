using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Client
{
    public class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] Button button;
        [SerializeField] GameObject emptyOverlay;

        private Item currentItem;
        private System.Action<Item> onClick;

        public void Setup(System.Action<Item> onClickCallback)
        {
            onClick = onClickCallback;
            button.onClick.AddListener(() =>
            {
                if (currentItem != null)
                {
                    onClick?.Invoke(currentItem);
                }
            });
        }

        public void SetItem(Item item)
        {
            currentItem = item;
            //icon.sprite = item.ItemData.icon;
            icon.enabled = true;
            emptyOverlay.SetActive(false);
        }

        public void Clear()
        {
            currentItem = null;
            icon.sprite = null;
            icon.enabled = false;
            emptyOverlay.SetActive(true);
        }
    }
}