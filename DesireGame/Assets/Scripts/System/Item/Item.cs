using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    // ����Ǵ� ������ ����, UI � �� ������ ����
    public class Item
    {
        private long _uid;
        private long _itemIndex;
        private ItemData _itemData;
        private List<(SystemEnum.eStats eStat, int increase)> _subStatList;

        public ItemData ItemData => _itemData;

        public Item(long itemID)
        {
            _itemIndex = itemID;
            InitItem();
        }

        public void InitItem()
        {
            _uid = ItemManager.Instance.GetNextID();
            _itemData = DataManager.Instance.GetData<ItemData>(_itemIndex);

            if (_itemData == null)
            {
                Debug.LogError("������ ������ ����, �ε��� Ȯ��");
                return;
            }
            _subStatList = ItemManager.Instance.GenerateSubStats(_itemData.subStatsCount);
        }

        public long GetID() => _uid;
    }
} 