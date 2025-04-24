using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    // SynergyManager에서 캐릭터의 중복 여부를 확인하기 위해 작성.
    // SynergyManager의 내장 클래스로 작성할까...?
    public class CharComparer : IEqualityComparer<CharBase>
    {
        public bool Equals(CharBase x, CharBase y)
        {
            if (x == null || y == null) return false;
            return x.Index == y.Index;
        }

        public int GetHashCode(CharBase obj)
        {
            return obj.Index.GetHashCode();
        }
    }

    public class CharUtil
    {
        public static CharBase GetNearestInList(CharBase clientChar, List<CharBase> targetList, int nTH = 0, bool inverse = false)
        {
            if(clientChar == false)
            {
                Debug.LogError("Client is null.");
                return null;
            }
            if(nTH > targetList.Count || nTH < 0)
            {
                Debug.LogError("Wrong Input");
                return null;
            }

            var distances = new List<(CharBase target, float dist)>(targetList.Count);
            Vector3 clientPosition = clientChar.CharTransform.position;

            for (int i = 0; i< targetList.Count; i++)
            {
                if (!targetList[i]) continue;
                float distSqr = (clientPosition - targetList[i].CharTransform.position).sqrMagnitude;
                distances.Add((targetList[i], distSqr));
            }

            distances.Sort((a, b) => a.dist.CompareTo(b.dist));
            if (inverse)
                distances.Sort((a, b) => b.dist.CompareTo(a.dist));
            else
                distances.Sort((a, b) => a.dist.CompareTo(b.dist));

            return distances[nTH].target;
        }

        public static eCharType GetEnemyType(eCharType clientType)
        {
            if (clientType == eCharType.ALLY)
                return eCharType.ENEMY;
            else if (clientType == eCharType.ENEMY)
                return eCharType.ALLY;
            else
                return eCharType.None;
        }
    }

}