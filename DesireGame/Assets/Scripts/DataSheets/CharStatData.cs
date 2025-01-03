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
    public partial class CharStatData : SheetData
    {
public long index; // 캐릭터Index
		public long HP; // HP
		public long Def; // Def
		public long SP; // SP
		public long Speed; // Speed
		public long Attack; // Attack
		

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

                    CharStatData data = new CharStatData();

                    
					if(values[0] == "")
					    data.index = default;
					else
					    data.index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.HP = default;
					else
					    data.HP = Convert.ToInt64(values[2]);
					
					if(values[3] == "")
					    data.Def = default;
					else
					    data.Def = Convert.ToInt64(values[3]);
					
					if(values[4] == "")
					    data.SP = default;
					else
					    data.SP = Convert.ToInt64(values[4]);
					
					if(values[5] == "")
					    data.Speed = default;
					else
					    data.Speed = Convert.ToInt64(values[5]);
					
					if(values[6] == "")
					    data.Attack = default;
					else
					    data.Attack = Convert.ToInt64(values[6]);
					
                    
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