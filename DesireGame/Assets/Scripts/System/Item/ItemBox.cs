using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    // 필드에 떨어질 아이템 구슬
    public class ItemBox : MonoBehaviour
    {
        private Item _item;
        private int _amount;
        private Collider _itemBoxCollider;
        private Transform _itemBoxTransform;

        private void Awake()
        {
            _itemBoxTransform = transform;
            _itemBoxCollider = GetComponent<Collider>();
        }


        public void WrapItem(Item item, int amount)
        {
            this._item = item;
            this._amount = amount;
        }

        private void OnMouseDown()
        {
            // 일단 클릭하면 아이템 정보 정해서 디버그 찍기
            Debug.Log($"아이템 획득 : {_item.GetID()} {_item.ItemData.nameStringCode}");
            Destroy(gameObject);
        }

    }
}