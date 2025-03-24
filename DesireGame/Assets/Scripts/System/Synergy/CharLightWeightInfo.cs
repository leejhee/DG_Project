using System.Collections.Generic;

namespace Client
{
    public struct CharLightWeightInfo
    {
        public long index;
        public long uid;
        public List<SystemEnum.eSynergy> synergyList;

        public readonly CharBase SpecifyCharBase()
        {
            return CharManager.Instance.GetFieldChar(uid);
        }
    }
}