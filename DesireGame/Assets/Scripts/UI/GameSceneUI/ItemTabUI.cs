using System;
using UnityEngine;

namespace Client
{
    public class ItemTabUI : MonoBehaviour
    {
        [SerializeField] Transform ItemGridPanel; // 네모난~ 아이템 슬롯들이 자리잡을 Vertical Layout Panel의 Transform(여기다가 Instantiate하겠지?)

        private void Start()
        {
            Inventory.Instance.OnItemChange += 아이템띄우기;
        }

        private void 아이템띄우기(ItemUIParameter itemParameter)
        {
            //대충 새로운 아이템 하나를 띄워주는 내용
        }
    }
}