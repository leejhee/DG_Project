using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class TileManager : Singleton<TileManager>
    {
        private Dictionary<int, TileObj> TileMap = new();

        #region 생성자
        TileManager() { }
        #endregion 

        public void SetTile(int index, TileObj tile)
        {
            if (TileMap.ContainsKey(index))
            {
                Debug.LogError($"TileManager.SetTile 타일 인덱스 : {index} 가 중복입니다.");
                return;
            }

            TileMap.Add(index, tile);
        }
        public TileObj GetTile(int index)
        {
            if (TileMap.ContainsKey(index))
            {
                return TileMap[index];
            }
            Debug.LogError($"TileManager.GetTile 타일 인덱스 : {index} 가 존재하지 않습니다.");
            return null;
        }

        // 타일 인덱스 idx1, idx2 를 교환한다.
        public void TradeChar(int idx1, int idx2)
        {
            if ((TileMap.ContainsKey(idx1) == false) || (TileMap.ContainsKey(idx2) == false))
            {
                return;
            }
            CharBase charBase1 = GetTile(idx1)?.GetChar();
            GetTile(idx1)?.SetChar(GetTile(idx2)?.GetChar());
            GetTile(idx2)?.SetChar(charBase1);
        }

    }
}