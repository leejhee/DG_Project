using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class TileManager : Singleton<TileManager>
    {
        private Dictionary<int, TileObj> TileMap = new();

        #region ������
        TileManager() { }
        #endregion 

        public void SetTile(int index, TileObj tile)
        {
            if (TileMap.ContainsKey(index))
            {
                Debug.LogError($"TileManager.SetTile Ÿ�� �ε��� : {index} �� �ߺ��Դϴ�.");
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
            Debug.LogError($"TileManager.GetTile Ÿ�� �ε��� : {index} �� �������� �ʽ��ϴ�.");
            return null;
        }

        // Ÿ�� �ε��� idx1, idx2 �� ��ȯ�Ѵ�.
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