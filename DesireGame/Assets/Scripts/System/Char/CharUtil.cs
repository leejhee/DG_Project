using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    // SynergyManager���� ĳ������ �ߺ� ���θ� Ȯ���ϱ� ���� �ۼ�.
    // SynergyManager�� ���� Ŭ������ �ۼ��ұ�...?
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
        // �ּ��ϱ�? GetNearest�� ������ ������ �ϸ� �����Ű���. �ϴ� �̷��� ������ �Ű������� �� ����...
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
    }

}