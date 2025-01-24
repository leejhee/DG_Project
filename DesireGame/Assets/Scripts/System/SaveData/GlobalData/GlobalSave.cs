using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    // static�� ����ȭ �ȵǾ� �̷��� �ۼ�..
    [System.Serializable]
    public class GlobalSave
    {
        public List<GameSave> IngameSaves;

        public List<PastExploreRecord> PastRecords;

        public List<ItemUnlocked> ItemUnlockList;

        public List<CharUnlocked> CharUnlockList;

        //��Ƽ��Ʈ�� Ŭ���� ¥�� �ϼ���

        public GlobalSave()
        {
            IngameSaves = new List<GameSave>();
            PastRecords = new List<PastExploreRecord>();
            ItemUnlockList = new List<ItemUnlocked>();
            CharUnlockList = new List<CharUnlocked>();
        }
    }
}