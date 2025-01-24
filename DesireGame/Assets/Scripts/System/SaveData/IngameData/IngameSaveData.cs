using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    [System.Serializable]
    public class GameSave
    {
        public int credit;

        public List<CharFieldSave> CurrentFieldChar;

        public int maxCharCount;

        public List<CharItemData> CurrentTotalItem;

        public GameSave()
        {
            credit = 0;
            CurrentFieldChar = new List<CharFieldSave>();
            maxCharCount = 1; // 매직넘버 괜찮을지...
            CurrentTotalItem = new List<CharItemData>();
            
        }

        // 데이터 갱신 후  보유 아티팩트 작업 하자.

        //////////////////////////////////////////////////////////////////////////
        /* 후순위 작업
            StoreData.
        - 상점 관련(일단 후순위. 상점 기획 나온 뒤 작업 요망)
            - 상점 레벨
            - 상점에 올려진 기물 or 아이템 등

            StageData.
        - 맵(맵 어떻게 만들어지는 지 기획 나온 뒤 작업 요망)
            - 던전 내 맵
            - 현재 층
            - 현재 위치
            - 던전 내 특정 방의 탐사 유무
            - ex) 3번째 방은 탐사 완료, 6번째 방은 미탐사 등
            - 이벤트 방문 여부
            - ex) 1회성 이벤트와 상호작용 했었는가?
        */
    }
}