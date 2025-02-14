using System.Collections.Generic;

namespace Client
{
    // SynergyManager에서 캐릭터의 중복 여부를 확인하기 위해 작성.
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
}