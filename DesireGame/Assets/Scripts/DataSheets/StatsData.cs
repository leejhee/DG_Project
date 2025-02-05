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
    public partial class StatsData : SheetData
    {
public long Index; // charID
		public int AD; // 공격력
		public int AP; // 주문력
		public int HP; // 체력
		public int attackSpeed; // 공격속도(천분율)
		public int critChance; // 치명타 확률(만분율)
		public int cirtDamage; // 치명타 피해(만분율)
		public int damageIncrease; // 피해량 증가(만분율)
		public int bonusDamage; // 추가 피해
		public int defense; // 방어력
		public int magicResist; // 마법 방어력
		public int Range; // 사거리
		public float moveSpeed; // 이동속도
		public int startMana; // 시작마나
		public int maxMana; // 최대마나
		

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

                    StatsData data = new StatsData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.AD = default;
					else
					    data.AD = Convert.ToInt32(values[2]);
					
					if(values[3] == "")
					    data.AP = default;
					else
					    data.AP = Convert.ToInt32(values[3]);
					
					if(values[4] == "")
					    data.HP = default;
					else
					    data.HP = Convert.ToInt32(values[4]);
					
					if(values[5] == "")
					    data.attackSpeed = default;
					else
					    data.attackSpeed = Convert.ToInt32(values[5]);
					
					if(values[6] == "")
					    data.critChance = default;
					else
					    data.critChance = Convert.ToInt32(values[6]);
					
					if(values[7] == "")
					    data.cirtDamage = default;
					else
					    data.cirtDamage = Convert.ToInt32(values[7]);
					
					if(values[8] == "")
					    data.damageIncrease = default;
					else
					    data.damageIncrease = Convert.ToInt32(values[8]);
					
					if(values[9] == "")
					    data.bonusDamage = default;
					else
					    data.bonusDamage = Convert.ToInt32(values[9]);
					
					if(values[10] == "")
					    data.defense = default;
					else
					    data.defense = Convert.ToInt32(values[10]);
					
					if(values[11] == "")
					    data.magicResist = default;
					else
					    data.magicResist = Convert.ToInt32(values[11]);
					
					if(values[12] == "")
					    data.Range = default;
					else
					    data.Range = Convert.ToInt32(values[12]);
					
					if(values[13] == "")
					    data.moveSpeed = default;
					else
					    data.moveSpeed = Convert.ToSingle(values[13]);
					
					if(values[14] == "")
					    data.startMana = default;
					else
					    data.startMana = Convert.ToInt32(values[14]);
					
					if(values[15] == "")
					    data.maxMana = default;
					else
					    data.maxMana = Convert.ToInt32(values[15]);
					

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