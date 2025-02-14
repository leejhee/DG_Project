using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Client
{
    /// <summary> 
    /// ������ �Ŵ��� (Sheet ������ ����)
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        /// �ε��� �� �ִ� DataTable (Table ����  Key1 ������ ID�� Key2�� ���)
        private Dictionary<string, Dictionary<long, SheetData>> _cache = new Dictionary<string, Dictionary<long, SheetData>>();

        #region Singleton
        private DataManager()
        { }
        #endregion

        #region ���� ������

        // �÷��̾� ��ġ����
        private Dictionary<SystemEnum.eScene, Vector3> _positionMap = new Dictionary<SystemEnum.eScene, Vector3>();
        public Dictionary<SystemEnum.eScene, Vector3> PositionMap => _positionMap;

        // ��������
        private Dictionary<string, Dictionary<SystemEnum.eLocalize, string>> _localizeStringCodeMap = new();
        public Dictionary<string, Dictionary<SystemEnum.eLocalize, string>> LocalizeStringCodeMap => _localizeStringCodeMap;

        public SystemEnum.eLocalize Localize { get; set; } = SystemEnum.eLocalize.KOR;

        #endregion

        // ������ �ε� �� �ൿ ����
        public Action DoAfterLoadActon;
        // �ε尡 �������� Ȯ��
        public bool EndLoad { get; private set; } = false;

        public override void Init()
        {
            DataLoad();
            ObjectManager.Instance.ObjectDataLoad();
            DoAfterDataLoad();
        }
        public void DataLoad()
        {
            // ���� ����� ������ SheetData�� ��ӹ޴� ��� Ÿ���� ã��
            var sheetDataTypes = Assembly.GetExecutingAssembly().GetTypes()
                                         .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SheetData)));

            List<SheetData> instances = new List<SheetData>();

            foreach (var type in sheetDataTypes)
            {

                // �� Ÿ�Կ� ���� �ν��Ͻ� ����
                SheetData instance = (SheetData)Activator.CreateInstance(type);
                if (instance == null)
                {
                    continue;
                }
                Dictionary<long, SheetData> sheet = instance.LoadData();

                if (_cache.ContainsKey(type.Name) == false)
                {
                    _cache.Add(type.Name, sheet);
                }

                SetTypeData(type.Name);
            }
            EndLoad = true;
        }
        public void DoAfterDataLoad()
        {
            if (DoAfterLoadActon == null)
                return;

            DoAfterLoadActon.Invoke();
            DoAfterLoadActon = null;
        }
        public T GetData<T>(long Index) where T : SheetData
        {
            string key = typeof(T).ToString();
            key = key.Replace("Client.", "");
            if (!_cache.ContainsKey(key))
            {
                Debug.LogError($"{key} ������ ���̺��� �������� �ʽ��ϴ�.");
                return null;
            }
            if (!_cache[key].ContainsKey(Index))
            {
                Debug.LogError($"{key} �����Ϳ� ID {Index}�� �������� �ʽ��ϴ�.");
                return null;
            }
            T returnData = _cache[key][Index] as T;
            if (returnData == null)
            {
                Debug.LogError($"{key} �����Ϳ� ID {Index}�� ���������� {key}Ÿ������ ��ȯ �����߽��ϴ�.");
                return null;

            }
            
            return returnData;
        }

        public void SetData<T>(int id, T data) where T : SheetData
        {
            string key = typeof(T).ToString();
            key = key.Replace("Client.", "");

            if (_cache.ContainsKey(key))
            {
                Debug.LogWarning($"{key} ������ ���̺��� �̹� �����մϴ�.");
            }
            else
            {
                _cache.Add(key, new Dictionary<long, SheetData>());
            }

            if (_cache[key].ContainsKey(id))
            {
                Debug.LogWarning($"{key} Ÿ�� ID: {id} Į���� �̹� �����մϴ�. !(����) ���� �� ������ Į���� ������ �� �����ϴ�!");
            }
            else 
            {
                _cache[key].Add(id, data);
            }
        }
        public List<SheetData> GetDataList<T>() where T : SheetData
        {
            string typeName = typeof(T).Name;
            if (_cache.ContainsKey(typeName) == false)
            {
                Debug.LogWarning($"DataManager : {typeName} Ÿ�� �����Ͱ� �������� �ʽ��ϴ�.");

                return null;
            }
            // �׳� _cache[typeName] �� �������� �ʰ� Linq�� �̿��ϸ� �ٸ� List�� �����ϹǷ�
            // List���� ��ü ����, Sort ��� ���� ������ ������ ���� �� �ִ�.
            // �ٸ� ��ü ���� �����͸� �ٲٴ°� ������ ���� �� �ִµ�
            // �츮 �����ʹ� readonly �ϱ� �� ������ ���� - �� �ƴϳ�!?!?!?!?!? �ʹ��� �����ѵ���!?!?!?
            // ��� Linq�� �������� ����� �����ϹǷ� Update���� Linq ����� ����
            return _cache[typeName].Values.ToList();
        }

#if UNITY_EDITOR
        // ������ ������(�����Ϳ����� ���)
        public Dictionary<long, SheetData> GetDictionary(string typeName)
        {
            if (_cache.ContainsKey(typeName) == false)
            {
                Debug.LogWarning($"DataManager : {typeName} Ÿ�� �����Ͱ� �������� �ʽ��ϴ�.");
                return null;
            }
            return _cache[typeName];
        }
        public void ClearCache()
        {
            _cache.Clear();
            _positionMap.Clear();
            _localizeStringCodeMap.Clear();
        }
#endif
         #region ���� ������

        // ���� ������ ����
        private void SetTypeData(string data)
        {
            if (typeof(CharPositionData).ToString().Contains(data)) { SetCharPositionData(); return; }
            if (typeof(StringCodeData).ToString().Contains(data)) { SetStringCodeData(); return; }
        }
        // �÷��̾� ��ġ����
        private void SetCharPositionData()
        {
            string key = typeof(CharPositionData).Name;
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

            Dictionary<long, SheetData> stringCodeMap = _cache[key];
            if(stringCodeMap == null)
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
}