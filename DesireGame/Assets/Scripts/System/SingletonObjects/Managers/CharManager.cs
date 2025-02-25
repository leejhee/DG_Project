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
        private Transform _charRoot = null;
        public Transform CharRoot
        {
            get 
            {
                if (_charRoot == null)
                {
                    var gm = new GameObject { name = "UnitRoot" };
                    _charRoot = gm.transform;
                }
                return _charRoot;
            }
        }
        
        public long SelectedCharIndex { get; private set; } = 1;
        // 고유 ID 생성
        public long GetNextID() => _nextID++;

        // 특정 타입의 캐릭터 수가 0이 되었을 때 발생하는 이벤트
        public event Action<Type> OnCharTypeEmpty;

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
            if (!_cache[myType].ContainsKey(id))
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
            CheckTypeEmpty(myType);
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

        public void Clear(CharBase charBase)
        {
            if (charBase == false)
                return;
            
            eCharType charType = charBase.CharData.charType;
            Clear(eCharTypeToType(charType), charBase.GetID());
        }

        public Type eCharTypeToType(eCharType charType)
        {
            switch (charType)
            {
                case eCharType.ALLY : return typeof(CharPlayer);
                case eCharType.ENEMY: return typeof(CharMonster);
            }
            return null;
        }
        public eCharType TypeToECharType(Type type)
        {
            var typeName = type.Name;

            if (typeof(CharPlayer).Name == typeName) return eCharType.ALLY;
            if (typeof(CharMonster).Name == typeName) return eCharType.ENEMY;
            
            return eCharType.None;
        }


        public CharBase CharGenerate(CharParameter charParam)
        {
            CharBase charBase = Instance.CharGenerate(charParam.CharIndex);
            charBase.transform.position = charParam.GeneratePos;
            return charBase;
        }

        // 타일 시스템
        public CharBase CharGenerate(CharTileParameter charParam)
        {
            CharBase charBase = Instance.CharGenerate(charParam.CharIndex);
            TileManager.Instance.SetChar(charParam.TileIndex, charBase);
            return charBase;
        }

        public CharBase CharGenerate(long charIndex)
        {
            CharData charData = DataManager.Instance.GetData<CharData>(charIndex);
            if (charData == null)
            {
                Debug.LogWarning($"CharFactory : {charIndex} 의 CharIndex를 찾을 수 없음");
                return null;
            }
            GameObject gameObject = ObjectManager.Instance.Instantiate($"Char/{charData.charPrefab}", CharRoot);
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

        /// <summary>
        /// 가장 가까운 적 가져오기
        /// </summary>
        /// <param name="ClientChar"></param>
        /// <returns></returns>
        public CharBase GetNearestEnemy(CharBase ClientChar, int nTH=0)
        {
            // TODO : N번째 selection으로 고쳐서 구현하기.

            eCharType clientType = ClientChar.GetCharType();
            Vector3 clientPosition = ClientChar.CharTransform.position;
            var enemyDict = new Dictionary<long, CharBase>();

            if (clientType == eCharType.ALLY)
            {
                enemyDict = _cache[typeof(CharMonster)];
            }
            else if (clientType == eCharType.ENEMY)
            {
                enemyDict = _cache[typeof(CharPlayer)];
            }

            CharBase nearestEnemy = null;
            float minDistanceSqr = float.MaxValue;  // 제곱 거리 비교를 위해

            foreach (var kvp in enemyDict)
            {
                CharBase enemy = kvp.Value;
                Vector3 enemyPosition = enemy.CharTransform.position;
                float distanceSqr = (clientPosition - enemyPosition).sqrMagnitude; // 거리 제곱

                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }

        


        /// <summary>
        /// _cache 안에 있는 캐릭터 AI 켜기
        /// </summary>
        public void WakeAllCharAI()
        {
            foreach (var kvp in _cache)
            {
                foreach(var unit in kvp.Value)
                {
                    Debug.Log($"AI - uid: {unit.Value.GetID()} 캐릭터 이름: {unit.Value.CharData.charName}");
                    unit.Value.AISwitch(true);
                }
            }
        }

        public void ClearAllChar()
        {
            if (_cache == null)
                return;
            foreach (var typeList in _cache.Values)
            {
                if (typeList == null)
                    continue;

                List<long> charIDList = new();
                foreach (var charBase in typeList.Values)
                {
                    charIDList.Add(charBase.GetID());
                }
                foreach (var charID in charIDList)
                {
                    typeList[charID].Dead();
                }
            }
        }


        private void CheckTypeEmpty(Type type)
        {
            if (_cache.ContainsKey(type) && _cache[type].Count == 0)
            {
                Debug.Log($"{type} 타입의 캐릭터가 0명이 됨.");
                OnCharTypeEmpty?.Invoke(type);
            }
        }

    }
}