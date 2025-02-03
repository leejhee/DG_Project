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
    public partial class SkillData : SheetData
    {
public long Index; // ID
		public string nameStringCode; // 이름 스트링 코드
		public string skillTimeLineName; // 스킬 애니메이션
		public long functionIndex; // 펑션 인덱스
		

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

                    SkillData data = new SkillData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.nameStringCode = default;
					else
					    data.nameStringCode = Convert.ToString(values[1]);
					
					if(values[3] == "")
					    data.skillTimeLineName = default;
					else
					    data.skillTimeLineName = Convert.ToString(values[3]);
					
					if(values[4] == "")
					    data.functionIndex = default;
					else
					    data.functionIndex = Convert.ToInt64(values[4]);
					

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