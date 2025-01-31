using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// Char ��Ʈ�ѿ� �Ŵ���
    /// Char ���� �� ��Ʈ�� ���� ��ü�� �ּҴ���
    /// </summary>
    public class CharManager : Singleton<CharManager>
    {
        // �����ϴ� Char (Char Type�� Key1 Char ID�� Key2�� ���)
        private Dictionary<Type, Dictionary<long, CharBase>> _cache = new Dictionary<Type, Dictionary<long, CharBase>>();
        // ���� ID ���� 
        private long _nextID = 0;
        #region ������
        CharManager() { }
        #endregion
        public long SelectedCharIndex { get; private set; } = 1;
        // ���� ID ����
        public long GetNextID() => _nextID++;

        // �ʵ� �� ĳ����, ���� ���ٿ�(Ű�� uid)
        private Dictionary<long, CharPlayer> _playerDict = new Dictionary<long, CharPlayer>();
        private Dictionary<long, CharMonster> _monsterDict = new Dictionary<long, CharMonster>();

       public T GetChar<T>(long ID) where T : CharBase
       {
            var key = typeof(T);

            if (!_cache.ContainsKey(key))
            {
                Debug.LogWarning($"{typeof(T).ToString()} Ÿ���� ã�� �� ����");
                return null;
            }
            if (_cache[key].ContainsKey(ID))
            {
                Debug.LogWarning($"{key} Ÿ���� ID: {ID}�� ã�� �� ����");
                return null;
            }
            T findChar = _cache[key][ID] as T;
            if (findChar == null)
            {
                Debug.LogWarning($"{key} Ÿ���� ID: {ID}�� {key} Ÿ������ ��ȯ �Ұ�");
                return null;
            }
            return findChar;
       }

        public bool SetChar<T>(T data) where T : CharBase
        {
            var key = typeof(T);
            if (!_cache.ContainsKey(typeof(T)))
            {
                _cache.Add(key, new Dictionary<long, CharBase>());
            }
            if (_cache[key].ContainsKey(data.GetID()))
            {
                Debug.LogWarning($"{key} Ÿ���� ID: {data.GetID()}�� �̹� ������");
                return false;
            }
            _cache[key].Add(data.GetID(),data);
            return true;
        }

        public bool Clear(Type myType,long id)
        {
            if (!_cache.ContainsKey(myType))
            {
                Debug.LogWarning($"{myType.ToString()} Ÿ���� ã�� �� ���� ���� ����");
                return false;
            }
            if (_cache[myType].ContainsKey(id))
            {
                Debug.LogWarning($"{myType.ToString()} Ÿ���� ID: {id}�� ã�� �� ���� ���� ����");
                return false;
            }
            var findChar = _cache[myType][id];
            if (findChar == null)
            {
                Debug.LogWarning($"{myType} Ÿ���� id: {id}�� {myType} Ÿ������ ��ȯ �Ұ� ���� ����");
                return false;
            }
            _cache[myType].Remove(id);
            return true;
        }

        public bool Clear(long id)
        {
            foreach(var typeData in _cache)
            {
                Dictionary<long, CharBase> typeDic = typeData.Value;
                
                if (typeDic == null)
                    continue;

                if (typeDic.ContainsKey(id))
                {
                    typeDic.Remove(id);
                }
            }

            return true;
        }

        public CharBase CharGenerate(CharParameter charParam)
        {
            CharData charData = DataManager.Instance.GetData<CharData>(charParam.CharIndex);
            if (charData == null)
            {
                Debug.LogWarning($"CharFactory : {charParam.CharIndex} �� CharIndex�� ã�� �� ����");
                return null;
            }
            GameObject gameObject = ObjectManager.Instance.Instantiate($"Char/{charData.charPrefab}");
            if (gameObject == null)
            {
                Debug.LogWarning($"CharFactory : {charData.charPrefab} �� charPrefab�� ã�� �� ����");
                return null;
            }

            CharBase charBase = gameObject.GetComponent<CharBase>();
            return charBase;
        }

        public CharBase SelectedPlayableCharGenerate()
        {
            CharParameter charParam = new CharParameter();
            charParam.CharIndex = SelectedCharIndex;
            charParam.Scene = SceneManager.NowScene;
            if (DataManager.Instance.PositionMap.ContainsKey(charParam.Scene))
            {
                charParam.GeneratePos = DataManager.Instance.PositionMap[charParam.Scene];
            }
            CharBase charBase = CharGenerate(charParam);
            if(charBase.CharCamaraPos != null)
            {
                Camera.main.transform.parent = charBase.CharCamaraPos.transform;
                Camera.main.transform.localPosition = Vector3.zero;
                Camera.main.transform.localEulerAngles = Vector3.zero;

            }

            return charBase;
        }

        /// <summary>
        /// �ʵ� �� ���� �Ǵ� �÷��̾� ĳ���� ���
        /// �ϴ� ���� ���� �ÿ��� ȣ��(���� ��ϵ� �� �ؾ���.)</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool RegisterFieldChar<T>(T unit) where T : CharBase
        {
            if(unit is CharPlayer)
            {
                if (_playerDict.ContainsKey(unit.GetID()))
                {
                    Debug.LogError($"uid {unit.GetID()} �̹� ������");
                    return false;
                }
                _playerDict.Add(unit.GetID(), unit as CharPlayer);
                return true;
            }

            if(unit is CharMonster)
            {
                if (_monsterDict.ContainsKey(unit.GetID()))
                {
                    Debug.LogError($"uid {unit.GetID()} �̹� ������");
                    return false;
                }
                _monsterDict.Add(unit.GetID(), unit as CharMonster);
                return true;
            }

            return false;
        }

        /// <summary>
        /// �ʵ忡 �ִ� Char ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool ClearFieldChar<T>(long uid) where T  : CharBase
        {
            if(typeof(T) == typeof(CharPlayer))
            {
                if (!_playerDict.ContainsKey(uid))
                {
                    Debug.LogError($"�ش� uid {uid} ����");
                    return false;
                }
                _playerDict.Remove(uid);
                return true;
            }

            if(typeof(T) == typeof(CharMonster))
            {
                if (!_monsterDict.ContainsKey(uid))
                {
                    Debug.LogError($"�ش� uid {uid} ����");
                    return false;
                }
                _monsterDict.Remove(uid);
                return true;
            }

            return false;
        }

        /// <summary>
        /// �ʵ忡 �ִ� ���� ĳ���� ��������
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public CharMonster GetFieldMonster(long uid)
        {
            if (!_monsterDict.ContainsKey(uid))
            {
                Debug.LogError($"uid {uid} ���� ����");
                return null;
            }
            return _monsterDict[uid];
        }

        /// <summary>
        /// �ʵ忡 �ִ� �÷��̾� ĳ���� ��������
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public CharPlayer GetFieldPlayer(long uid)
        {
            if(! _playerDict.ContainsKey(uid))
            {
                Debug.LogError($"uid {uid} ���� ����");
                return null;
            }
            return _playerDict[uid];
        }

        public CharBase GetFieldChar(long uid)
        {
            foreach (var dict in _cache.Values)
            {
                if (dict.ContainsKey(uid))
                {
                    return dict[uid];
                }
            }
            return null;
        }
    }
}