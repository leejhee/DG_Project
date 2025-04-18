using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Client
{
    public class ItemIconUI : MonoBehaviour
    {
        // 드래그 앤 드롭이 가능한 아이템 아이콘 UI
        [SerializeField] Image icon;
        [SerializeField] Button button;
        [SerializeField] TMPro.TextMeshProUGUI iconReplacedByID; // TODO : 아이콘 이미지 나오면 바꿈, 일단 임시

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

        public void SetIcon(Item item)
        {
            currentItem = item;
            //icon.sprite = item.ItemData.icon;
            icon.enabled = true;
            iconReplacedByID.text = $"{item.GetID()}";
        }

        public void Clear()
        {
            currentItem = null;
            icon.sprite = null;
            icon.enabled = false;
            iconReplacedByID.text = null;
        }

    }
}