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
    public partial class CharData : SheetData
    {
public long Index; // charID
		public string charName; // 캐릭터 이름
		public string charPrefab; // 캐릭터 프리팹
		
		public SystemEnum.eCharTier charTier; // 캐릭터 등급
		public long specIndex; // 기본스펙
		
		public SystemEnum.eSynergy synergy1; // 시너지1
		
		public SystemEnum.eSynergy synergy2; // 시너지2
		
		public SystemEnum.eSynergy synergy3; // 시너지3
		public long autoAttack; // 스킬1(평타)
		public long skillAttack; // 스킬2(고유 스킬)
		public List<long> func; // 초기 기능(패시브)
		
		public SystemEnum.eCharType charType; // 포트레이트 이미지 string
		

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

                    CharData data = new CharData();

                    
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
					    data.charTier = default;
					else
					    data.charTier = (SystemEnum.eCharTier)Enum.Parse(typeof(SystemEnum.eCharTier), values[3]);
					
					if(values[4] == "")
					    data.specIndex = default;
					else
					    data.specIndex = Convert.ToInt64(values[4]);
					
					if(values[5] == "")
					    data.synergy1 = default;
					else
					    data.synergy1 = (SystemEnum.eSynergy)Enum.Parse(typeof(SystemEnum.eSynergy), values[5]);
					
					if(values[6] == "")
					    data.synergy2 = default;
					else
					    data.synergy2 = (SystemEnum.eSynergy)Enum.Parse(typeof(SystemEnum.eSynergy), values[6]);
					
					if(values[7] == "")
					    data.synergy3 = default;
					else
					    data.synergy3 = (SystemEnum.eSynergy)Enum.Parse(typeof(SystemEnum.eSynergy), values[7]);
					
					if(values[8] == "")
					    data.autoAttack = default;
					else
					    data.autoAttack = Convert.ToInt64(values[8]);
					
					if(values[9] == "")
					    data.skillAttack = default;
					else
					    data.skillAttack = Convert.ToInt64(values[9]);
					
					ListStr = values[10].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var funcData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.func = funcData;
					
					if(values[11] == "")
					    data.charType = default;
					else
					    data.charType = (SystemEnum.eCharType)Enum.Parse(typeof(SystemEnum.eCharType), values[11]);
					

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