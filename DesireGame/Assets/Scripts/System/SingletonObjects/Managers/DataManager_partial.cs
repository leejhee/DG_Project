using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
namespace Client
{
    public partial class DataManager
    {
        #region ���� ������

        // �÷��̾� ��ġ����
        private Dictionary<eScene, Vector3> _positionMap = new Dictionary<eScene, Vector3>();
        public Dictionary<eScene, Vector3> PositionMap => _positionMap;

        // ��������
        private Dictionary<string, Dictionary<eLocalize, string>> _localizeStringCodeMap = new();
        public Dictionary<string, Dictionary<eLocalize, string>> LocalizeStringCodeMap => _localizeStringCodeMap;

        // �������� ������ 
        private Dictionary<int, List<MonsterSpawnInfo>> _monsterSpawnStageMap = new();
        public Dictionary<int, List<MonsterSpawnInfo>> MonsterSpawnStageMap => _monsterSpawnStageMap;

        // �ó��� �� ĳ���� �з�
        private Dictionary<eSynergy, List<CharData>> _synergyCharacterMap = new();
        public Dictionary<eSynergy, List<CharData>> SynergyCharacterMap => _synergyCharacterMap;

        // �ó��� ���� function Data
        private Dictionary<eSynergy, Dictionary<int, List<SynergyData>>> _synergyDataMap = new();
        public Dictionary<eSynergy, Dictionary<int, List<SynergyData>>> SynergyDataMap => _synergyDataMap;

        // �ó��� Ʈ���� ��ųʸ�
        private Dictionary<eSynergy, FunctionData> _synergyTriggerMap = new();
        public Dictionary<eSynergy, FunctionData> SynergyTriggerMap => _synergyTriggerMap;
        
        // ������ ������
        private Dictionary<eItemTier, List<ItemData>> _itemDataMap = new();
        public Dictionary<eItemTier, List<ItemData>> ItemDataMap => _itemDataMap;


        public eLocalize Localize { get; set; } = eLocalize.KOR;

        #endregion

        #region ���� ������

        // ���� ������ ����
        private void SetTypeData(string data)
        {
            if (typeof(CharPositionData).ToString().Contains(data)) { SetCharPositionData(); return; }
            if (typeof(StringCodeData).ToString().Contains(data)) { SetStringCodeData(); return; }
            if (typeof(MonsterSpawnData).ToString().Contains(data)) { SetMonsterSpawnData(); return; }
            if (typeof(CharData).ToString().Contains(data)) { SetSynergyCharMap(); return; }
            if (typeof(SynergyData).ToString().Contains(data)) { SetSynergyMappingData(); return; }
            if (typeof(FunctionData).ToString().Contains(data)) { SetSynergyTriggerMap(); return; }
            if (typeof(ItemData).ToString().Contains(data)) { SetItemDataMap(); return; }

        }
        // �÷��̾� ��ġ����
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

        // ��Ʈ�� �ڵ�
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

        // �������� ���� ������ 
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

        private void SetSynergyCharMap()
        {
            string key = typeof(CharData).Name;
            if (!_cache.ContainsKey(key))
                return;

            var charDict = _cache[key];
            if (charDict is null) return;

            foreach(var kvp in charDict)
            {
                var _char = kvp.Value as CharData;
                var synergies = new List<eSynergy>() 
                { _char.synergy1, _char.synergy2, _char.synergy3 };

                foreach(var synergy in synergies)
                {
                    if (!SynergyCharacterMap.ContainsKey(synergy))
                        SynergyCharacterMap.Add(synergy, new List<CharData>());
                    if (!SynergyCharacterMap[synergy].Contains(_char))
                        SynergyCharacterMap[synergy].Add(_char);
                }
            }

        }



        // �ó��� ������ ������ ������.
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
                if (!_synergyDataMap.ContainsKey(synergy.synergy))
                    _synergyDataMap.Add(synergy.synergy, new Dictionary<int, List<SynergyData>>());
                if(!_synergyDataMap[synergy.synergy].ContainsKey(synergy.synergyCount))
                    _synergyDataMap[synergy.synergy].Add(synergy.synergyCount, new List<SynergyData>());
                if (!_synergyDataMap[synergy.synergy][synergy.synergyCount].Contains(synergy))
                    _synergyDataMap[synergy.synergy][synergy.synergyCount].Add(synergy);
            }
        }

        // �ó����� Ʈ���� �з�
        private void SetSynergyTriggerMap()
        {
            string key = typeof(FunctionData).Name;
            if (!_cache.ContainsKey(key))
                return;
            
            var triggerDict = _cache[key];
            if(triggerDict is null) return;

            foreach(var kvp in triggerDict)
            {
                var trigger = kvp.Value as FunctionData;
                if(trigger.function == eFunction.SYNERGY_TRIGGER)
                {
                    if (trigger.input1 >= (long)eSynergy.eMax) continue;
                        var synergyType = (eSynergy)trigger.input1;
                    if(!_synergyTriggerMap.ContainsKey(synergyType))
                        _synergyTriggerMap.Add(synergyType, trigger);
                }
            }


        }


        // ������ Ƽ� ������
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
            return $"(��Ʈ�� �ڵ尡 �����ϴ�!){stringCode}";
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