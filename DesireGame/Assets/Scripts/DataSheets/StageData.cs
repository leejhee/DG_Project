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
    public partial class StageData : SheetData
    {
public long Index; // StageID
		public string SceneName; // Scene이름
		public string SceneStartBGM; // Scene 시작 BGM 이름
		

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

                    StageData data = new StageData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.SceneName = default;
					else
					    data.SceneName = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.SceneStartBGM = default;
					else
					    data.SceneStartBGM = Convert.ToString(values[3]);
					

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