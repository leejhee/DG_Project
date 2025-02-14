using System.Collections.Generic;

namespace Client
{
    // SynergyManager���� ĳ������ �ߺ� ���θ� Ȯ���ϱ� ���� �ۼ�.
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