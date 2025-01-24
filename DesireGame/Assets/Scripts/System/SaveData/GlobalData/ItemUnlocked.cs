using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    [System.Serializable]
    public class ItemUnlocked
    {
        public CharItemData targetItem;
        public bool unlocked;
    }

    [System.Serializable]
    public class CharUnlocked
    {
        public CharData targetChar;
        public bool unlocked;
    }

    [System.Serializable]
    public class ArtifactUnlocked
    {
        //public CharData targetChar; Ŭ���� ����� �ϼ���
        public bool unlocked;
    }
}