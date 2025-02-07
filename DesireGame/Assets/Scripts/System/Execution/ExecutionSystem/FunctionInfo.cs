using static Client.SystemEnum;
using System.Collections.Generic;

namespace Client
{
    public class FunctionInfo
    {
        private Dictionary<eFunction, List<FunctionBase>> _functionBaseDic = new Dictionary<eFunction, List<FunctionBase>>(); // ±â´É 
        public Dictionary<eFunction, List<FunctionBase>> FunctionBaseDic => _functionBaseDic;

        public void Init()
        {
            for (eFunction i = 0; i < eFunction.MaxCount; i++)
            {
                _functionBaseDic[i] = new List<FunctionBase>();
            }
        }

    }
}