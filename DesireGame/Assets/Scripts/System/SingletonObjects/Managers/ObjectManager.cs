using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    /// <summary>
    /// Prefab 등을 Load, Cache 함
    /// </summary>
    public class ObjectManager : Singleton<ObjectManager>
    {
        /// <summary> 로드한 적 있는 object cache 기본적으로 경로를 Key로 이용</summary>
        Dictionary<string, Object> _cache = new Dictionary<string, Object>();

        #region 생성자
        ObjectManager() { }
        #endregion 생성자

        /// <summary>
        /// 프리팹 Path 를 얻는다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetPrefabPath(string path) => $"Prefabs/{path}";

        /// <summary> 
        /// Resources.Load로 불러오기
        /// </summary>
        public T Load<T>(string path) where T : Object
        {
            string name = path;

            Object obj;

            if (_cache == null)
            {
                _cache = new Dictionary<string, Object>();
            }

            //캐시에 존재 -> 캐시에서 반환
            if (_cache.TryGetValue(name, out obj))
                return obj as T;

            //캐시에 없음 -> 로드하여 캐시에 저장 후 반환
            obj = Resources.Load<T>(path);
            _cache.Add(name, obj);

            return obj as T;
        }

        /// <summary> GameObject 생성 </summary>
        public GameObject Instantiate(string path, Transform parent = null) => Instantiate<GameObject>(path, parent);

        /// <summary> GameObject 생성 </summary>
        public GameObject Instantiate(GameObject gm, Transform parent = null) => GameObject.Instantiate(gm, parent);

        /// <summary> GameObject 생성 </summary>
        public GameObject Instantiate(GameObject gm, Vector3 position) => GameObject.Instantiate(gm, position, Quaternion.identity);
        /// <summary> AudioClip Load </summary>
        public AudioClip LoadAudioClip(string path) => Instance.Load<AudioClip>($"Sounds/{path}");

        /// <summary> T type object 생성 </summary>
        public T Instantiate<T>(string path, Transform parent = null) where T : UnityEngine.Object
        {
            T prefab = Load<T>(GetPrefabPath(path));
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab : {path}");
                return null;
            }

            T instance = null;
            if (parent == null)
            {

                instance = UnityEngine.Object.Instantiate<T>(prefab);
            }
            else
            {
                instance = UnityEngine.Object.Instantiate<T>(prefab, parent);
            }
            instance.name = prefab.name;


            return instance;
        }

        /// <summary>
        /// Cache 초기화 (맵 이동, 메모리 초과 상황)
        /// </summary>
        public void Clear()
        {
            if (_cache == null)
                _cache = new Dictionary<string, Object>();

            foreach (var kvp in _cache)
            {
                Resources.UnloadAsset(kvp.Value);
            }
            _cache.Clear();
        }

        /// <summary>
        /// Clear by Value
        /// </summary>
        /// <param name="clearObject"></param>
        /// <returns></returns>
        public bool Clear(Object clearObject)
        {
            if (clearObject == null)
            {
                Debug.LogError("ClearObject is null.");
                return false;
            }

            string keyToRemove = null;

            // 사전을 순회하며 값이 일치하는 항목을 찾음
            foreach (var kvp in _cache)
            {
                if (kvp.Value == clearObject)
                {
                    keyToRemove = kvp.Key;
                    break; // 일치하는 항목을 찾으면 루프를 중단
                }
            }

            if (keyToRemove != null || keyToRemove == "")
            {
                return Clear(keyToRemove);
            }
            return false;
        }

        /// <summary>
        /// Clear by Key
        /// </summary>
        /// <param name="clearKey"></param>
        /// <returns></returns>
        public bool Clear(string clearKey)
        {
            if (!clearKey.StartsWith("Prefabs/"))
            {
                clearKey = GetPrefabPath(clearKey);
            }
            if (_cache.TryGetValue(clearKey, out Object obj))
            {
                Resources.UnloadAsset(obj);
                _cache.Remove(clearKey);
                return true;
            }
            else
            {
                Debug.LogWarning($"No resource found with name: {clearKey}");
                return false;
            }
        }

    }
}