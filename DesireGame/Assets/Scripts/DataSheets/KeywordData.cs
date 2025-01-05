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
    public partial class KeywordData : SheetData
    {
public long Index; // keywordID
		public string keyword; // 키워드
		
		public SystemEnum.EKeywordType[] keywordType; // 키워드의 종류
		public int stratSentenceIndex; // 전략 노트에서의 문장 번호
		public int stratBlankIndex; // 빈칸 위치
		public int charSentenceIndex; // 캐릭터 노트에서의 문장 번호
		public int charBlankIndex; // 빈칸 위치
		

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

                    KeywordData data = new KeywordData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.keyword = default;
					else
					    data.keyword = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.keywordType = default;
					else
					    data.keywordType = (SystemEnum.EKeywordType[])Enum.Parse(typeof(SystemEnum.EKeywordType[]), values[2]);
					
					if(values[3] == "")
					    data.stratSentenceIndex = default;
					else
					    data.stratSentenceIndex = Convert.ToInt32(values[3]);
					
					if(values[4] == "")
					    data.stratBlankIndex = default;
					else
					    data.stratBlankIndex = Convert.ToInt32(values[4]);
					
					if(values[5] == "")
					    data.charSentenceIndex = default;
					else
					    data.charSentenceIndex = Convert.ToInt32(values[5]);
					
					if(values[6] == "")
					    data.charBlankIndex = default;
					else
					    data.charBlankIndex = Convert.ToInt32(values[6]);
					

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