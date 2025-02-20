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
    public partial class SynergyData : SheetData
    {
public long Index; // synergyID
		public List<long> functionList; // 기능
		
		public SystemEnum.eSynergy synergyType; // 시너지 종류
		public int levelThresholds; // 해당 버프를 위한 문턱인원수
		
		public SystemEnum.eSynergyLevel level; // 무슨레벨인지
		public string levelIcon; // 아이콘
		

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

                    SynergyData data = new SynergyData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					ListStr = values[1].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var functionListData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.functionList = functionListData;
					
					if(values[2] == "")
					    data.synergyType = default;
					else
					    data.synergyType = (SystemEnum.eSynergy)Enum.Parse(typeof(SystemEnum.eSynergy), values[2]);
					
					if(values[3] == "")
					    data.levelThresholds = default;
					else
					    data.levelThresholds = Convert.ToInt32(values[3]);
					
					if(values[4] == "")
					    data.level = default;
					else
					    data.level = (SystemEnum.eSynergyLevel)Enum.Parse(typeof(SystemEnum.eSynergyLevel), values[4]);
					
					if(values[5] == "")
					    data.levelIcon = default;
					else
					    data.levelIcon = Convert.ToString(values[5]);
					

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