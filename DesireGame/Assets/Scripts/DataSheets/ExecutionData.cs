using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Client
{
    [Serializable]
    public partial class ExecutionData : SheetData
    {
public long Index; // ID
		
		public SystemEnum.eFunction functionType; // 기능
		
		public SystemEnum.eStats effectState; // 스테이트 타입
		public long duration; // 시간(MS)천분율
		public long input1; // input1
		public long input2; // input2
		public long input3; // input3
		public long input4; // input4
		

        public override Dictionary<long, SheetData> LoadData()
        {
            var dataList = new Dictionary<long, SheetData>();

            string ListStr = null;
			int line = 0;
            TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{this.GetType().Name}");
            try
			{            
                string csvContent = csvFile.text;
                var lines = Regex.Split(csvContent, @"\r?\n");
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = Regex.Split(lines[i].Trim(),
                                        @",(?=(?:[^""\[\]]*(?:""[^""]*""|[\[][^\]]*[\]])?)*[^""\[\]]*$)");
  
                    for (int j = 0; j < values.Length; j++)
                    {
                        values[j] = Regex.Replace(values[j], @"^""|""$", "");
                    }
                    line = i;

                    ExecutionData data = new ExecutionData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[3] == "")
					    data.functionType = default;
					else
					    data.functionType = (SystemEnum.eFunction)Enum.Parse(typeof(SystemEnum.eFunction), values[3]);
					
					if(values[4] == "")
					    data.effectState = default;
					else
					    data.effectState = (SystemEnum.eStats)Enum.Parse(typeof(SystemEnum.eStats), values[4]);
					
					if(values[5] == "")
					    data.duration = default;
					else
					    data.duration = Convert.ToInt64(values[5]);
					
					if(values[6] == "")
					    data.input1 = default;
					else
					    data.input1 = Convert.ToInt64(values[6]);
					
					if(values[7] == "")
					    data.input2 = default;
					else
					    data.input2 = Convert.ToInt64(values[7]);
					
					if(values[8] == "")
					    data.input3 = default;
					else
					    data.input3 = Convert.ToInt64(values[8]);
					
					if(values[9] == "")
					    data.input4 = default;
					else
					    data.input4 = Convert.ToInt64(values[9]);
					

                    dataList[data.Index] = data;
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