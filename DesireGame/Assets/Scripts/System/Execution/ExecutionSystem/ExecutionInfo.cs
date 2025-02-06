using static Client.SystemEnum;
using System.Collections.Generic;

namespace Client
{
    public class ExecutionInfo
    {
        private Dictionary<eFunction, List<ExecutionBase>> _executionBaseDic = new Dictionary<eFunction, List<ExecutionBase>>(); // ±â´É 
        public Dictionary<eFunction, List<ExecutionBase>> ExecutionBaseDic => _executionBaseDic;

        public void Init()
        {
            for (eFunction i = 0; i < eFunction.MaxCount; i++)
            {
                _executionBaseDic[i] = new List<ExecutionBase>();
            }
        }

    }
}