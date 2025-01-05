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
    public partial class CharacterSpawnData : SheetData
    {
public long Index; // spawnID
		public long charID; // 캐릭터ID
		
		public SystemEnum.EGameData whichGame; // 게임 정보
		
		public SystemEnum.ERound roundInfo; // 라운드 정보
		public long startPositionX; // 시작 위치x
		public long startPositionY; // 시작 위치y
		

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

                    CharacterSpawnData data = new CharacterSpawnData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.charID = default;
					else
					    data.charID = Convert.ToInt64(values[2]);
					
					if(values[3] == "")
					    data.whichGame = default;
					else
					    data.whichGame = (SystemEnum.EGameData)Enum.Parse(typeof(SystemEnum.EGameData), values[3]);
					
					if(values[4] == "")
					    data.roundInfo = default;
					else
					    data.roundInfo = (SystemEnum.ERound)Enum.Parse(typeof(SystemEnum.ERound), values[4]);
					
					if(values[5] == "")
					    data.startPositionX = default;
					else
					    data.startPositionX = Convert.ToInt64(values[5]);
					
					if(values[6] == "")
					    data.startPositionY = default;
					else
					    data.startPositionY = Convert.ToInt64(values[6]);
					

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