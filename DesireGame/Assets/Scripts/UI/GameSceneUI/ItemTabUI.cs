using System;
using UnityEngine;

namespace Client
{
    public class ItemTabUI : MonoBehaviour
    {
        [SerializeField] Transform ItemGridPanel; // �׸�~ ������ ���Ե��� �ڸ����� Vertical Layout Panel�� Transform(����ٰ� Instantiate�ϰ���?)

        private void Start()
        {
            Inventory.Instance.OnItemChange += �����۶���;
        }

        private void �����۶���(ItemUIParameter itemParameter)
        {
            //���� ���ο� ������ �ϳ��� ����ִ� ����
        }
    }
}