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
    public partial class SkillData : SheetData
    {
public long index; // 스킬Index
		public string skillName; // 스킬명
		
		public SystemEnum.eSkillType skillType; // 스킬타입
		public List<long> skillExecutionList; // 스킬시작Execution
		public string skillPortraitImage; // 스킬아이콘이미지
		public string skillTimeLineName; // 스킬타임라인명
		

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

                    SkillData data = new SkillData();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.skillName = default;
					else
					    data.skillName = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.skillType = default;
					else
					    data.skillType = (SystemEnum.eSkillType)Enum.Parse(typeof(SystemEnum.eSkillType), values[3]);
					
					ListStr = values[4].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var skillExecutionListData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.skillExecutionList = skillExecutionListData;
					
					if(values[5] == "")
					    data.skillPortraitImage = default;
					else
					    data.skillPortraitImage = Convert.ToString(values[5]);
					
					if(values[6] == "")
					    data.skillTimeLineName = default;
					else
					    data.skillTimeLineName = Convert.ToString(values[6]);
					

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