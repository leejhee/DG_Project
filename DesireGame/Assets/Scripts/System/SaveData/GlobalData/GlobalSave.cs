using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    // static은 직렬화 안되어 이렇게 작성..
    [System.Serializable]
    public class GlobalSave
    {
        public List<GameSave> IngameSaves;

        public List<PastExploreRecord> PastRecords;

        public List<ItemUnlocked> ItemUnlockList;

        public List<CharUnlocked> CharUnlockList;

        //아티팩트는 클래스 짜고 하세요

        public GlobalSave()
        {
            IngameSaves = new List<GameSave>();
            PastRecords = new List<PastExploreRecord>();
            ItemUnlockList = new List<ItemUnlocked>();
            CharUnlockList = new List<CharUnlocked>();
        }
    }
}