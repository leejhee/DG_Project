using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    // �ʵ忡 ������ ������ ����
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
            // �ϴ� Ŭ���ϸ� ������ ���� ���ؼ� ����� ���
            Debug.Log($"������ ȹ�� : {_item.GetID()} {_item.ItemData.nameStringCode}");
            Destroy(gameObject);
        }

    }
}