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
    public partial class FunctionData : SheetData
    {
public long Index; // ID
		
		public SystemEnum.eFunction function; // 기능
		
		public SystemEnum.eStats statsType; // 스테이트 타입
		
		public SystemEnum.eDamageType damageType; // 데미지 타입
		
		public SystemEnum.eCCType CCType; // CC 타입
		public long time; // 시간(MS)천분율
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

                    string[] values = CSVParser.Parse(lines[i].Trim());

                    line = i;

                    FunctionData data = new FunctionData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[3] == "")
					    data.function = default;
					else
					    data.function = (SystemEnum.eFunction)Enum.Parse(typeof(SystemEnum.eFunction), values[3]);
					
					if(values[4] == "")
					    data.statsType = default;
					else
					    data.statsType = (SystemEnum.eStats)Enum.Parse(typeof(SystemEnum.eStats), values[4]);
					
					if(values[5] == "")
					    data.damageType = default;
					else
					    data.damageType = (SystemEnum.eDamageType)Enum.Parse(typeof(SystemEnum.eDamageType), values[5]);
					
					if(values[6] == "")
					    data.CCType = default;
					else
					    data.CCType = (SystemEnum.eCCType)Enum.Parse(typeof(SystemEnum.eCCType), values[6]);
					
					if(values[7] == "")
					    data.time = default;
					else
					    data.time = Convert.ToInt64(values[7]);
					
					if(values[8] == "")
					    data.input1 = default;
					else
					    data.input1 = Convert.ToInt64(values[8]);
					
					if(values[9] == "")
					    data.input2 = default;
					else
					    data.input2 = Convert.ToInt64(values[9]);
					
					if(values[10] == "")
					    data.input3 = default;
					else
					    data.input3 = Convert.ToInt64(values[10]);
					
					if(values[11] == "")
					    data.input4 = default;
					else
					    data.input4 = Convert.ToInt64(values[11]);
					

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