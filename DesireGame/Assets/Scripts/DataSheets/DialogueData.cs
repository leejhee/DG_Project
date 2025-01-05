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
    public partial class DialogueData : SheetData
    {
public long Index; // dialougeID
		
		public SystemEnum.EGameData whichGame; // 게임 이름
		
		public SystemEnum.ERound roundInfo; // 라운드 정보
		public int roomID; // 장소ID
		public int charID; // 캐릭터ID
		
		public SystemEnum.ECharImg charImg; // 캐릭터 표정
		public string dialouge; // 무슨 말
		public List<long> keyword; // 키워드
		public int startH; // 시작 시
		public int startM; // 시작 분
		public int endH; // 끝 시
		public int endM; // 끝 분
		
		public SystemEnum.EDialougeEff dialougeEff; // 특수효과
		public string dialougeSfx; // 사운드경로
		

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

                    DialogueData data = new DialogueData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.whichGame = default;
					else
					    data.whichGame = (SystemEnum.EGameData)Enum.Parse(typeof(SystemEnum.EGameData), values[1]);
					
					if(values[2] == "")
					    data.roundInfo = default;
					else
					    data.roundInfo = (SystemEnum.ERound)Enum.Parse(typeof(SystemEnum.ERound), values[2]);
					
					if(values[3] == "")
					    data.roomID = default;
					else
					    data.roomID = Convert.ToInt32(values[3]);
					
					if(values[5] == "")
					    data.charID = default;
					else
					    data.charID = Convert.ToInt32(values[5]);
					
					if(values[7] == "")
					    data.charImg = default;
					else
					    data.charImg = (SystemEnum.ECharImg)Enum.Parse(typeof(SystemEnum.ECharImg), values[7]);
					
					if(values[8] == "")
					    data.dialouge = default;
					else
					    data.dialouge = Convert.ToString(values[8]);
					
					ListStr = values[10].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var keywordData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.keyword = keywordData;
					
					if(values[11] == "")
					    data.startH = default;
					else
					    data.startH = Convert.ToInt32(values[11]);
					
					if(values[12] == "")
					    data.startM = default;
					else
					    data.startM = Convert.ToInt32(values[12]);
					
					if(values[13] == "")
					    data.endH = default;
					else
					    data.endH = Convert.ToInt32(values[13]);
					
					if(values[14] == "")
					    data.endM = default;
					else
					    data.endM = Convert.ToInt32(values[14]);
					
					if(values[15] == "")
					    data.dialougeEff = default;
					else
					    data.dialougeEff = (SystemEnum.EDialougeEff)Enum.Parse(typeof(SystemEnum.EDialougeEff), values[15]);
					
					if(values[16] == "")
					    data.dialougeSfx = default;
					else
					    data.dialougeSfx = Convert.ToString(values[16]);
					

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