using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data;
using System.Linq;

namespace Client
{
    public partial class ExecutionData : SheetData
    {
public long index; // ExecutionIndex
		
		public SystemEnum.eExecutionGroupType GroupType; // 대분류
		public int Priority; // 우선순위 (클수록 높은 우선순위)
		
		public SystemEnum.eExecutionType ExecutionType; // ExecutionType
		
		public SystemEnum.eState State; // 영향받는 State
		
		public SystemEnum.eExecutionCondition ExecutionCondition; // 조건부Execution
		public List<long> ValueList; // ValueList
		public List<string> ValueStrList; // ValueStrList
		public long ExecutionTime; // Execution지속시간10000분율( -1 은 무한지속 ) 
		public List<long> ChainExecution; // 연계Execution
		
		public SystemEnum.eExecutionCondition ChainExecutionCondition; // 연계Execution조건
		

        public override Dictionary<long, SheetData> LoadData()
        {
            var dataList = new Dictionary<long, SheetData>();

            string ListStr = null;
			int line = 0;
            TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{this.GetType().Name}");
            try
			{            
                string csvContent = csvFile.text;
                string[] lines = csvContent.Split('\n');
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = lines[i].Trim().Split(',');
                    line = i;

                    ExecutionData data = new ExecutionData();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.GroupType = default;
					else
					    data.GroupType = (SystemEnum.eExecutionGroupType)Enum.Parse(typeof(SystemEnum.eExecutionGroupType), values[2]);
					
					if(values[3] == "")
					    data.Priority = default;
					else
					    data.Priority = Convert.ToInt32(values[3]);
					
					if(values[4] == "")
					    data.ExecutionType = default;
					else
					    data.ExecutionType = (SystemEnum.eExecutionType)Enum.Parse(typeof(SystemEnum.eExecutionType), values[4]);
					
					if(values[5] == "")
					    data.State = default;
					else
					    data.State = (SystemEnum.eState)Enum.Parse(typeof(SystemEnum.eState), values[5]);
					
					if(values[6] == "")
					    data.ExecutionCondition = default;
					else
					    data.ExecutionCondition = (SystemEnum.eExecutionCondition)Enum.Parse(typeof(SystemEnum.eExecutionCondition), values[6]);
					
					ListStr = values[7].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var ValueListData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.ValueList = ValueListData;
					
					ListStr = values[8].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var ValueStrListData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToString(x)).ToList();
					data.ValueStrList = ValueStrListData;
					
					if(values[9] == "")
					    data.ExecutionTime = default;
					else
					    data.ExecutionTime = Convert.ToInt64(values[9]);
					
					ListStr = values[10].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var ChainExecutionData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.ChainExecution = ChainExecutionData;
					
					if(values[11] == "")
					    data.ChainExecutionCondition = default;
					else
					    data.ChainExecutionCondition = (SystemEnum.eExecutionCondition)Enum.Parse(typeof(SystemEnum.eExecutionCondition), values[11]);
					

                    dataList[data.index] = data;
                }

                return dataList;
            }
			catch (Exception e)
			{
				Debug.LogError($"{this.GetType().Name}의 {line}전후로 데이터 문제 발생");
				return new Dictionary<long, SheetData>();
			}
        }
    }
}