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
    public partial class DataManager : Singleton<DataManager>
    {
        /// �ε��� �� �ִ� DataTable (Table ����  Key1 ������ ID�� Key2�� ���)
        private Dictionary<string, Dictionary<long, SheetData>> _cache = new Dictionary<string, Dictionary<long, SheetData>>();

        #region Singleton
        private DataManager()
        { }
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
         

    }
}