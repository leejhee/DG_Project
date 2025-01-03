using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// Char 컨트롤용 매니저
    /// Char 게임 내 컨트롤 가능 객체의 최소단위
    /// </summary>
    public class CharManager : Singleton<CharManager>
    {
        // 존재하는 Char (Char Type을 Key1 Char ID를 Key2로 사용)
        private Dictionary<Type, Dictionary<long, CharBase>> _cache = new Dictionary<Type, Dictionary<long, CharBase>>();
        // 고유 ID 생성 
        private long _nextID = 0;
        #region 생성자
        CharManager() { }
        #endregion
        public long SelectedCharIndex { get; private set; } = 1;
        // 고유 ID 생성
        public long GetNextID() => _nextID++;

       public T GetChar<T>(long ID) where T : CharBase
       {
            var key = typeof(T);

            if (!_cache.ContainsKey(key))
            {
                Debug.LogWarning($"{typeof(T).ToString()} 타입을 찾을 수 없음");
                return null;
            }
            if (_cache[key].ContainsKey(ID))
            {
                Debug.LogWarning($"{key} 타입의 ID: {ID}을 찾을 수 없음");
                return null;
            }
            T findChar = _cache[key][ID] as T;
            if (findChar == null)
            {
                Debug.LogWarning($"{key} 타입의 ID: {ID}을 {key} 타입으로 변환 불가");
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
                Debug.LogWarning($"{key} 타입의 ID: {data.GetID()}가 이미 존재함");
                return false;
            }
            _cache[key].Add(data.GetID(),data);
            return true;
        }

        public bool Clear(Type myType,long id)
        {
            if (!_cache.ContainsKey(myType))
            {
                Debug.LogWarning($"{myType.ToString()} 타입을 찾을 수 없음 삭제 실패");
                return false;
            }
            if (_cache[myType].ContainsKey(id))
            {
                Debug.LogWarning($"{myType.ToString()} 타입의 ID: {id}을 찾을 수 없음 삭제 실패");
                return false;
            }
            var findChar = _cache[myType][id];
            if (findChar == null)
            {
                Debug.LogWarning($"{myType} 타입의 id: {id}을 {myType} 타입으로 변환 불가 삭제 실패");
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
                Debug.LogWarning($"CharFactory : {charParam.CharIndex} 의 CharIndex를 찾을 수 없음");
                return null;
            }
            GameObject gameObject = ObjectManager.Instance.Instantiate($"Char/{charData.charPrefab}");
            if (gameObject == null)
            {
                Debug.LogWarning($"CharFactory : {charData.charPrefab} 의 charPrefab을 찾을 수 없음");
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
    }
}