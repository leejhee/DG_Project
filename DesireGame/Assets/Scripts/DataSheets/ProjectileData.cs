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
    public partial class ProjectileData : SheetData
    {
public long Index; // ID
		
		public SystemEnum.eProjectileTargetType target; // 타겟
		
		public SystemEnum.eProjectilePathType path; // 날아가는 방식
		
		public SystemEnum.eProjectileRangeType rangeType; // 폭발 범위타입
		public int penetrationCount; // 관통 수
		

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

                    ProjectileData data = new ProjectileData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.target = default;
					else
					    data.target = (SystemEnum.eProjectileTargetType)Enum.Parse(typeof(SystemEnum.eProjectileTargetType), values[1]);
					
					if(values[2] == "")
					    data.path = default;
					else
					    data.path = (SystemEnum.eProjectilePathType)Enum.Parse(typeof(SystemEnum.eProjectilePathType), values[2]);
					
					if(values[3] == "")
					    data.rangeType = default;
					else
					    data.rangeType = (SystemEnum.eProjectileRangeType)Enum.Parse(typeof(SystemEnum.eProjectileRangeType), values[3]);
					
					if(values[4] == "")
					    data.penetrationCount = default;
					else
					    data.penetrationCount = Convert.ToInt32(values[4]);
					

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