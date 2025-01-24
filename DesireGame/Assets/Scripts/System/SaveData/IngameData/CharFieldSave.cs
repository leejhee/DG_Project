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

        // 필드의 위치(좌표 추상화하여 표현)
        public int x;
        public int y;
    }
}