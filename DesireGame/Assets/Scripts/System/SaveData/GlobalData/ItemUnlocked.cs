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
        //public CharData targetChar; 클래스 만들고 하세요
        public bool unlocked;
    }
}