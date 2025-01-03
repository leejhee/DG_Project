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
    public partial class CharData : SheetData
    {
public long index; // 캐릭터Index
		public string charName; // 캐릭터 이름
		
		public SystemEnum.eCharType charType; // 캐릭터타입
		public List<long> charSkillList; // 케릭터스킬
		public string charPrefab; // 캐릭터프리팹명
		public List<long> charDefaultBuffList; // 캐릭터 기본 버프
		public string charPortraitImage; // 캐릭터아이콘이미지
		public long charStatId; // 캐릭터 Stat
		

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

					if (values.Length <= 0)
						continue;

					if (values[0].Contains("#"))
						continue;

                    CharData data = new CharData();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.charName = default;
					else
					    data.charName = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.charType = default;
					else
					    data.charType = (SystemEnum.eCharType)Enum.Parse(typeof(SystemEnum.eCharType), values[3]);
					
					ListStr = values[4].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var charSkillListData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.charSkillList = charSkillListData;
					
					if(values[5] == "")
					    data.charPrefab = default;
					else
					    data.charPrefab = Convert.ToString(values[5]);
					
					ListStr = values[6].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var charDefaultBuffListData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.charDefaultBuffList = charDefaultBuffListData;
					
					if(values[7] == "")
					    data.charPortraitImage = default;
					else
					    data.charPortraitImage = Convert.ToString(values[7]);
					
					if(values[8] == "")
					    data.charStatId = default;
					else
					    data.charStatId = Convert.ToInt64(values[8]);
					
                    
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