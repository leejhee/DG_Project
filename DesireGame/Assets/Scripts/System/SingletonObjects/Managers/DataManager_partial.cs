using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
namespace Client
{
    public partial class DataManager
    {
        #region 개별 데이터

        // 플레이어 위치정보
        private Dictionary<eScene, Vector3> _positionMap = new Dictionary<eScene, Vector3>();
        public Dictionary<eScene, Vector3> PositionMap => _positionMap;

        // 번역정보
        private Dictionary<string, Dictionary<eLocalize, string>> _localizeStringCodeMap = new();
        public Dictionary<string, Dictionary<eLocalize, string>> LocalizeStringCodeMap => _localizeStringCodeMap;

        // 스테이지 데이터 
        private Dictionary<int, List<MonsterSpawnInfo>> _monsterSpawnStageMap = new();
        public Dictionary<int, List<MonsterSpawnInfo>> MonsterSpawnStageMap => _monsterSpawnStageMap;

        // 시너지 관련 function Data
        private Dictionary<eSynergy, Dictionary<int, SynergyData>> _synergyTriggerMap = new();
        public Dictionary<eSynergy, Dictionary<int, SynergyData>> SynergyTriggerMap => _synergyTriggerMap;

        // 아이템 데이터
        private Dictionary<eItemTier, List<ItemData>> _itemDataMap = new();
        public Dictionary<eItemTier, List<ItemData>> ItemDataMap => _itemDataMap;


        public eLocalize Localize { get; set; } = eLocalize.KOR;

        #endregion

        #region 개별 데이터

        // 개별 데이터 가공
        private void SetTypeData(string data)
        {
            if (typeof(CharPositionData).ToString().Contains(data)) { SetCharPositionData(); return; }
            if (typeof(StringCodeData).ToString().Contains(data)) { SetStringCodeData(); return; }
            if (typeof(MonsterSpawnData).ToString().Contains(data)) { SetMonsterSpawnData(); return; }
            if (typeof(SynergyData).ToString().Contains(data)) { SetSynergyMappingData(); return; }
            if (typeof(ItemData).ToString().Contains(data)) { SetItemDataMap(); return; }


        }
        // 플레이어 위치정보
        private void SetCharPositionData()
        {
            string key = typeof(CharPositionData).Name;
            if (_cache.ContainsKey(key) == false)
                return;
            Dictionary<long, SheetData> charPositionMap = _cache[key];
            if (charPositionMap == null)
            {
                return;
            }



            foreach (var posMap in charPositionMap.Values)
            {
                CharPositionData charPosition = posMap as CharPositionData;
                float xPos = (float)charPosition.xPos / SystemConst.PER_TEN_THOUSAND;
                float yPos = (float)charPosition.yPos / SystemConst.PER_TEN_THOUSAND;
                float zPos = (float)charPosition.zPos / SystemConst.PER_TEN_THOUSAND;

                Vector3 vector = new Vector3(xPos, yPos, zPos);
                _positionMap.Add(charPosition.mapScene, vector);
            }
        }

        // 스트링 코드
        private void SetStringCodeData()
        {
            string key = typeof(StringCodeData).Name;
            if (_cache.ContainsKey(key) == false)
                return;
            Dictionary<long, SheetData> stringCodeMap = _cache[key];
            if (stringCodeMap == null)
            {
                return;
            }

            foreach (var _stringCode in stringCodeMap.Values)
            {
                StringCodeData stringCode = _stringCode as StringCodeData;
                Dictionary<SystemEnum.eLocalize, string> keyValuePairs = new();
                keyValuePairs.Add(SystemEnum.eLocalize.KOR, stringCode.KOR);
                keyValuePairs.Add(SystemEnum.eLocalize.ENG, stringCode.ENG);

                _localizeStringCodeMap.Add(stringCode.StringCode, keyValuePairs);
            }
        }

        // 스테이지 몬스터 데이터 
        private void SetMonsterSpawnData()
        {
            string key = typeof(MonsterSpawnData).Name;
            if (_cache.ContainsKey(key) == false)
                return;
            Dictionary<long, SheetData> monsterSpawnMap = _cache[key];
            if (monsterSpawnMap == null)
            {
                return;
            }

            _monsterSpawnStageMap.Clear();

            foreach (var _monsterSpawn in monsterSpawnMap.Values)
            {
                MonsterSpawnData monsterSpawn = _monsterSpawn as MonsterSpawnData;
                if (_monsterSpawnStageMap.ContainsKey(monsterSpawn.StageNum) == false)
                {
                    _monsterSpawnStageMap.Add(monsterSpawn.StageNum, new List<MonsterSpawnInfo>());
                }
                MonsterSpawnInfo monsterSpawnInfo = new MonsterSpawnInfo(
                    monsterSpawn.StageNum,
                    monsterSpawn.MonsterID,
                    monsterSpawn.PositionIndex,
                    monsterSpawn.Index
                    );
                _monsterSpawnStageMap[monsterSpawn.StageNum].Add(monsterSpawnInfo);
            }
        }

        // 시너지 종류별 데이터 뭉텅이.
        private void SetSynergyMappingData()
        {
            string key =  typeof(SynergyData).Name;
            if (!_cache.ContainsKey(key))
                return;

            var synergyDict = _cache[key];
            if (synergyDict is null) return;

            foreach(var kvp in synergyDict)
            {
                var synergy = kvp.Value as SynergyData;
                if (!_synergyTriggerMap.ContainsKey(synergy.synergy))
                    _synergyTriggerMap.Add(synergy.synergy, new Dictionary<int, SynergyData>());
                if(!_synergyTriggerMap[synergy.synergy].ContainsKey(synergy.synergyCount))
                    _synergyTriggerMap[synergy.synergy].Add(synergy.synergyCount, synergy);
            }
        }

        // 아이템 티어별 데이터
        private void SetItemDataMap()
        {
            string key = typeof(ItemData).Name;
            if (_cache.ContainsKey(key) == false)
                return;

            var itemDict = _cache[key];
            if (itemDict is null) return;

            foreach (var kvp in itemDict)
            {
                var itemData = kvp.Value as ItemData;

                if (!_itemDataMap.ContainsKey(itemData.itemTier))
                    _itemDataMap.Add(itemData.itemTier, new List<ItemData>());
                if (!_itemDataMap[itemData.itemTier].Contains(itemData))
                    _itemDataMap[itemData.itemTier].Add(itemData);
            }
        }


        public static string GetStringCode(string stringCode)
        {
            if (Instance._localizeStringCodeMap.ContainsKey(stringCode))
            {
                if (Instance._localizeStringCodeMap[stringCode].ContainsKey(Instance.Localize))
                {
                    return Instance._localizeStringCodeMap[stringCode][Instance.Localize];
                }
            }
            return $"(스트링 코드가 없습니다!){stringCode}";
        }
        #endregion
    }

    public class MonsterSpawnInfo
    {
        public readonly int StageNum;
        public readonly long MonsterID;
        public readonly int PositionIndex;
        public readonly long SpawnID;
        public MonsterSpawnInfo (
            int stageNum, 
            long monsterID,
            int positionIndex,
            long spawnID
            )
        {
            StageNum = stageNum;
            MonsterID = monsterID;
            PositionIndex = positionIndex;
            SpawnID = spawnID;
        }
    }

}