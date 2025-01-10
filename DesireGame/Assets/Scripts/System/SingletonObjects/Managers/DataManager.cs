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


        #endregion

        public override void Init()
        {
            DataLoad();
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
                Debug.Log(type.Name);
                Dictionary<long, SheetData> sheet = instance.LoadData();

                if (_cache.ContainsKey(type.Name) == false)
                {
                    _cache.Add(type.Name, sheet);
                }
            }
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

#if UNITY_EDITOR
        // ������ ������(�����Ϳ����� ���)
        public Dictionary<long, SheetData> GetDictionary(string typeName)
        {            
            return _cache[typeName];
        }
#endif


    }
}