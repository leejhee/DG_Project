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
    public partial class CharItemData : SheetData
    {
public long Index; // 아이템Index
		public string itemName; // 아이템명
		public string itemPrefabName; // 아이템프리팹명
		public string itemSpriteName; // 아이템스프라이트명
		
		public SystemEnum.eItemType itemType; // 아이템 타입
		public bool itemDisposable; // 일회용여부
		public List<long> itemEffectExecutionList; // 아이템효과(Execution)
		public long skillIndex; // 사용가능스킬그룹
		

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

                    string[] values = Regex.Split(lines[i], @",(?=(?:[^""]*""[^""]*"")*[^""]*$)");
                    line = i;

                    CharItemData data = new CharItemData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.itemName = default;
					else
					    data.itemName = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.itemPrefabName = default;
					else
					    data.itemPrefabName = Convert.ToString(values[3]);
					
					if(values[4] == "")
					    data.itemSpriteName = default;
					else
					    data.itemSpriteName = Convert.ToString(values[4]);
					
					if(values[5] == "")
					    data.itemType = default;
					else
					    data.itemType = (SystemEnum.eItemType)Enum.Parse(typeof(SystemEnum.eItemType), values[5]);
					
					if(values[6] == "")
					    data.itemDisposable = default;
					else
					    data.itemDisposable = Convert.ToBoolean(values[6]);
					
					ListStr = values[7].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var itemEffectExecutionListData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.itemEffectExecutionList = itemEffectExecutionListData;
					
					if(values[8] == "")
					    data.skillIndex = default;
					else
					    data.skillIndex = Convert.ToInt64(values[8]);
					

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