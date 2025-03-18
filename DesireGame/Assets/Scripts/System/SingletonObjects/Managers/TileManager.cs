using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
using static Client.SystemConst;

namespace Client
{
    public class TileManager : Singleton<TileManager>
    {
        private Dictionary<int, TileObj> TileMap = new();

        private readonly float MaxDistSqrt = 10f;
        #region ������
        TileManager() { }
        #endregion 

        // ������......
        public void SubscribePlayerMove()
        {
            MessageManager.SubscribeMessage<PlayerMove>(this, VecToTileIndex);
        }

        public void SwitchTileCombatmode(bool isCombat)
        {
            foreach (var tile in TileMap.Values)
            {
                tile.SwitchCombatBehaviour(isCombat);
            }
        }

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

        // ĳ���� Ÿ�Ͽ� �ø���
        public void SetChar(int tileIndex, CharBase setChar)
        {
            // �� null �� �ƴ� == false��? MonoBehaviour fake null �̽� 
            if (setChar == false)
                return;

            if (TileMap.ContainsKey(tileIndex) == false)
            {
                Debug.LogError($"TileManager.SetChar Ÿ�� �ε��� : {tileIndex}�� �������� �ʴ� Ÿ�� �ε����Դϴ�.");
                return;
            }          

            var tile = TileMap[tileIndex];
            if(tile.TeamTile != setChar.GetCharType())
            {
                Debug.LogWarning("��� ������ �̵��� �� �����ϴ�.");
                return;
            }
            tile.SetChar(setChar);
        }

        public void VecToTileIndex(PlayerMove mas)
        {
            if (mas == null)
                return;

            int index = NearTileIndex(mas.moveChar.transform.position);
          
            if (index == -1 || index == mas.beforeTileIndex ||
                GetTile(index).TeamTile != mas.moveChar.GetCharType())
            {
                SetChar(mas.beforeTileIndex, mas.moveChar);
                return;
            }
            TradeChar(mas.beforeTileIndex, index);
        }

        public int NearTileIndex(Vector3 vector3)
        {
            if (vector3 == null) return -1;

            float final = float.MaxValue;
            int index = -1;

            if (TileMap == null) return -1;
            foreach (var tile in TileMap.Values)
            {
                var distile = vector3 - tile.transform.position;
                float dist = ((distile.x * distile.x) + (distile.z * distile.z));
                if (dist < final)
                {
                    final = dist;
                    index = tile.TileIndex;
                }
            }
            if (final > MaxDistSqrt)
            {
                return -1;
            }
            else
            {
                return index;
            }
        }

        #region Character Tile Movement Helper Method

        /// <summary> ĳ���� Ÿ�� ���� �Ʊ� �� ���� Ư�� ���� �ε������� ��ȯ </summary>
        public List<int> GetDemandingPositions(eCharType type, bool sameSide, int col)
        {
            List<int> results = new();
            int colOffset = 0;                    

            if (type == eCharType.ALLY)
            {
                colOffset += sameSide ? TILE_COL_COUNT - col : TILE_COL_COUNT + col - 1;
            }
            else if (type == eCharType.ENEMY)
            {
                colOffset += sameSide ? TILE_COL_COUNT + col - 1 : TILE_COL_COUNT - col;
            }

            int startIndex = colOffset * TILE_COL_OFFSET;
            for (int idx = startIndex; idx < startIndex + TILE_SIDE_OFFSET; idx++)
                results.Add(idx);

            return results;
        }

        /// <summary> ĳ���� Ÿ�� ���� �Ʊ�, ���� �� ����Ʈ�� ��ȯ </summary>
        public List<int> GetDemandingPositions(eCharType type, bool sameSide)
        {
            List<int> results = new();
            int startPoint = 0;
            if (type == eCharType.ALLY)
            {
                startPoint += sameSide ? 0 : TILE_SIDE_OFFSET;
            }
            else if (type == eCharType.ENEMY)
            {
                startPoint += sameSide ? TILE_SIDE_OFFSET : 0;
            }

            for(int idx = startPoint; idx < startPoint + TILE_SIDE_OFFSET; idx++)
                results.Add(idx);

            return results;
        }

        public List<int> FilterIndices(List<int> rawIndices)
        {
            List<int> indices = new();
            foreach(int i in rawIndices)
            {
                if (GetTile(i) == false || GetTile(i).Accessable == false) continue;
                indices.Add(i);
            }
           return indices;
        }

        public Vector3 GetFarthestPos(Vector3 charPos, List<int> candidates)
        {
            if(candidates == null || candidates.Count == 0)
            {
                Debug.LogError("�ĺ����� �̻� ����. ����� �ٶ��ϴ�.");
                return Vector3.zero;
            }

            int result = -1;
            float farthest = 0;
            foreach(var idx in candidates)
            {
                var originDist = (GetTile(idx).transform.position - charPos).sqrMagnitude;
                if(originDist > farthest)
                {
                    farthest = originDist;
                    result = idx;
                }
            }
            return GetTile(result).transform.position;
        } 

        
        public void TeleportAllyFarthest(CharBase client)
        {
            var demandingPositions = GetDemandingPositions(client.GetCharType(), true);
            var filteredPositions = FilterIndices(demandingPositions);
            var beforePos = client.CharTransform.position;
            client.CharTransform.position = GetFarthestPos(beforePos, filteredPositions);
            Debug.Log($"�ڷ���Ʈ �Ϸ�. {client.GetID()}�� {client.name}�� {beforePos}���� {client.CharTransform.position}���� �̵�");
        }

        #endregion
    }
}