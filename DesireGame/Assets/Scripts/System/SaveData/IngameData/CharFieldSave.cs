using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    [System.Serializable]
    public class CharFieldSave
    {
        public CharData fieldCharData;

        public List<CharItemData> equipedItems;

        // �ʵ��� ��ġ(��ǥ �߻�ȭ�Ͽ� ǥ��)
        public int x;
        public int y;
    }
}