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
        #region 생성자
        TileManager() { }
        #endregion 

        // 구린데......
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

        // 캐릭터 타일에 올리기
        public void SetChar(int tileIndex, CharBase setChar)
        {
            // 왜 null 이 아닌 == false냐? MonoBehaviour fake null 이슈 
            if (setChar == false)
                return;

            if (TileMap.ContainsKey(tileIndex) == false)
            {
                Debug.LogError($"TileManager.SetChar 타일 인덱스 : {tileIndex}는 존재하지 않는 타일 인덱스입니다.");
                return;
            }          

            var tile = TileMap[tileIndex];
            if(tile.TeamTile != setChar.GetCharType())
            {
                Debug.LogWarning("상대 팀으로 이동할 수 없습니다.");
                return;
            }
            tile.SetChar(setChar);

            // 뭔가 체크.
            {

            }
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

        /// <summary> 캐릭터 타입 기준 아군 및 적군 특정 열의 인덱스들을 반환 </summary>
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
            for (int idx = startIndex; idx < startIndex + TILE_COL_OFFSET; idx++)
                results.Add(idx);

            return results;
        }

        /// <summary> 캐릭터 타입 기준 아군, 적군 측 리스트를 반환 </summary>
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
                if (GetTile(i) == false || GetTile(i).Accessible == false) continue;
                indices.Add(i);
            }
            return indices;
        }

        public Vector3 GetFarthestPos(Vector3 charPos, List<int> candidates)
        {
            if(candidates == null || candidates.Count == 0)
            {
                Debug.LogError("후보군에 이상 생김. 디버깅 바랍니다.");
                return Vector3.zero;
            }

            int result = -1;
            int farthest = 0;
            foreach(var idx in candidates)
            {
                var originDist = (int)(GetTile(idx).transform.position - charPos).sqrMagnitude;
                if(originDist > farthest ||
                   (originDist == farthest && result/TILE_COL_OFFSET > idx/TILE_COL_OFFSET))
                {
                    farthest = originDist;
                    result = idx;
                }
                
            }
            return GetTile(result).transform.position;
        } 

        
        public void TeleportAllyFarthest(CharBase client)
        {
            var targetPositions = new List<int>();
            var startPoint = TileIndexToRowType[NearTileIndex(client.transform.position)] == eRowType.ALLY_FRONT
                ? TILE_COL_COUNT
                : 0;
            
            // 여기 수정해야함
            for (int i = 0; i < TILE_COL_COUNT; i++)
            {
                int targetCol = startPoint == 0 ? i + 1 : TILE_COL_COUNT - i;

                targetPositions = GetDemandingPositions(eCharType.ALLY, true, targetCol);
                targetPositions = FilterIndices(targetPositions);
    
                if (targetPositions.Count > 0)
                    break;
            }
            
            var beforePos = client.CharTransform.position;
            client.CharTransform.position = GetFarthestPos(beforePos, targetPositions);
            Debug.Log($"텔레포트 완료. {client.GetID()}번 {client.name}이 {beforePos}에서 {client.CharTransform.position}으로 이동");
        }
        
        
        
        #endregion
    }
}