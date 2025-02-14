using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Client
{
    public partial class DataManager
    {
        #region ���� ������

        // �÷��̾� ��ġ����
        private Dictionary<SystemEnum.eScene, Vector3> _positionMap = new Dictionary<SystemEnum.eScene, Vector3>();
        public Dictionary<SystemEnum.eScene, Vector3> PositionMap => _positionMap;

        // ��������
        private Dictionary<string, Dictionary<SystemEnum.eLocalize, string>> _localizeStringCodeMap = new();
        public Dictionary<string, Dictionary<SystemEnum.eLocalize, string>> LocalizeStringCodeMap => _localizeStringCodeMap;

        // �������� ������ 
        private Dictionary<int, List<MonsterSpawnInfo>> _monsterSpawnStageMap = new();

        public Dictionary<int, List<MonsterSpawnInfo>> MonsterSpawnStageMap => _monsterSpawnStageMap;

        public SystemEnum.eLocalize Localize { get; set; } = SystemEnum.eLocalize.KOR;

        #endregion

        #region ���� ������

        // ���� ������ ����
        private void SetTypeData(string data)
        {
            if (typeof(CharPositionData).ToString().Contains(data)) { SetCharPositionData(); return; }
            if (typeof(StringCodeData).ToString().Contains(data)) { SetStringCodeData(); return; }
            if (typeof(MonsterSpawnData).ToString().Contains(data)) { SetMonsterSpawnData(); return; }


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