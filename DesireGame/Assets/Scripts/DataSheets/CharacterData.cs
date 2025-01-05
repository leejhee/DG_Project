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
    public partial class CharacterData : SheetData
    {
public long Index; // charID
		public string charName; // 캐릭터 이름
		public string charPrefab; // 캐릭터 프리팹
		public string charDefaultImgPath; // 기본
		public string charSmileImgPath; // 웃는
		public string charSobImgPath; // 우는
		public string charRageImgPath; // 화난
		public string charWorryImgPath; // 난처
		public string charShockImgPath; // 놀람
		public List<string> charExtraImgPath; // 바리에이션
		

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

                    CharacterData data = new CharacterData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.charName = default;
					else
					    data.charName = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.charPrefab = default;
					else
					    data.charPrefab = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.charDefaultImgPath = default;
					else
					    data.charDefaultImgPath = Convert.ToString(values[3]);
					
					if(values[4] == "")
					    data.charSmileImgPath = default;
					else
					    data.charSmileImgPath = Convert.ToString(values[4]);
					
					if(values[5] == "")
					    data.charSobImgPath = default;
					else
					    data.charSobImgPath = Convert.ToString(values[5]);
					
					if(values[6] == "")
					    data.charRageImgPath = default;
					else
					    data.charRageImgPath = Convert.ToString(values[6]);
					
					if(values[7] == "")
					    data.charWorryImgPath = default;
					else
					    data.charWorryImgPath = Convert.ToString(values[7]);
					
					if(values[8] == "")
					    data.charShockImgPath = default;
					else
					    data.charShockImgPath = Convert.ToString(values[8]);
					
					ListStr = values[9].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var charExtraImgPathData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToString(x)).ToList();
					data.charExtraImgPath = charExtraImgPathData;
					

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