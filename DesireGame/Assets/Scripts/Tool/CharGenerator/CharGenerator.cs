#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Client
{
    public class CharGenerator : EditorWindow
    {
        Object SPUM_Object = null;

        Dictionary<long, SheetData> CharDict;       
        string[] Options;
        int selectedOptionIndex;

        string targetPath = "Assets/Resources/Prefabs/";

        [MenuItem("Tools/CharGenerator")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CharGenerator), false, "Character Generator");
        }

        void OnEnable()
        {
            DataManager.Instance.DataLoad();
            CharDict = DataManager.Instance.GetDictionary("CharData");
            List<string> options = new();
            foreach(var value in CharDict.Values)
            {
                CharData data = value as CharData;
                options.Add($"charName : {data.charName} - index : {data.Index}");
            }
            Options = options.ToArray();
        }

        void OnGUI()
        {
            GUILayout.Label("캐릭터 생성 툴", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SPUM 원형 캐릭터를 넣어주세요.");
            GUILayout.FlexibleSpace();
            SPUM_Object = EditorGUILayout.ObjectField(SPUM_Object, typeof(GameObject), false, GUILayout.MaxWidth(300));
            EditorGUILayout.EndHorizontal();
           
            EditorGUILayout.HelpBox("SPUM 원형 캐릭터에 캐릭터 필수 기능을 제작하여 넣습니다.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("어떤 캐릭터의 프리팹인가요?");
            GUILayout.FlexibleSpace();
            selectedOptionIndex = EditorGUILayout.Popup(selectedOptionIndex, Options, GUILayout.MaxWidth(300));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("프리팹 이름을 제외하고 입력해주세요.");
            GUILayout.FlexibleSpace();
            EditorGUILayout.TextField(targetPath, GUILayout.MaxWidth(500));
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("캐릭터 생성 또는 기능 추가", GUILayout.Width(300)))
            {
                GeneratorChar();
            }   
        }

        /// <summary>
        /// 이후 기능 추가가 필요할 수 있음 반드시 이를 생각하고 작업할 것
        /// </summary>
        private void GeneratorChar()
        {
            long selectedId = new List<long>(CharDict.Keys)[selectedOptionIndex];
            CharData targetData = CharDict[selectedId] as CharData;

            if (SPUM_Object is null)
            {
                Debug.LogError("먼저 선택하고 누르셨나요?");
                return;
            }

            #region COPY SELECTED SPUM ASSET
            string assetPath = AssetDatabase.GetAssetPath(SPUM_Object);
            string savePath = targetPath + targetData.charPrefab + ".prefab";
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("에셋이 아니래요!");               
                return;
            }
            else if (File.Exists(savePath))
            {
                Debug.LogError($"중복되는 이름의 파일이 프리팹 폴더에 존재합니다. {targetPath}를 확인해주세요.");
                return;
            }
            #endregion

            #region LOAD COPIED ASSET AND EDIT

            GameObject SPUMPrefab = PrefabUtility.InstantiatePrefab(PrefabUtility.SaveAsPrefabAsset(SPUM_Object as GameObject, savePath)) as GameObject;
            if (!SPUMPrefab)
            {
                Debug.LogError("복사하고 로드했는데 null? 뭔가 잘못됨. 여기에 걸리면 안됨.");
                return;
            }
            CharBase[] existingCharBases = SPUMPrefab.GetComponents<CharBase>();
            foreach (var charBase in existingCharBases)
            {
                DestroyImmediate(charBase);
            }

            CharBase newCharBase = CharFactory.AddCharBase(targetData, SPUMPrefab);
            SerializedObject serialized = new SerializedObject(newCharBase);
            SerializedProperty Index = serialized.FindProperty("_index");
            Index.longValue = targetData.Index;
            serialized.ApplyModifiedProperties();

            CharFactory.AddDescendant(SPUMPrefab);

            PrefabUtility.ApplyPrefabInstance(SPUMPrefab, InteractionMode.AutomatedAction);

            #endregion

            #region RENAME ASSET WITH DATA
            // 프리팹 에셋 이름 바꾸기
            SPUMPrefab.name = targetData.charPrefab;
            string renameResult = AssetDatabase.RenameAsset(savePath, targetData.charPrefab);
            if (string.IsNullOrEmpty(renameResult))
                Debug.Log($"성공적으로 프리팹이 {SPUMPrefab.name}으로 등록되었습니다! 저장 경로에서 확인해주세요.");
            else
                Debug.LogError($"{renameResult}의 오류입니다. {targetPath}에 같은 이름의 에셋이 없는지 확인하세요.");

            AssetDatabase.SaveAssets();
            #endregion
        }

    }

    /// <summary> 에디터에서 CharBase 상속하는 캐릭터 뽑는 용 </summary>
    public static class CharFactory
    {
        public static CharBase AddCharBase(CharData data, GameObject go)
        {
            switch (data.charType)
            {
                case SystemEnum.eCharType.NPC:
                    return go.AddComponent<CharNPC>();
                case SystemEnum.eCharType.Player:
                    return go.AddComponent<CharPlayer>();
                case SystemEnum.eCharType.Monster:
                    return go.AddComponent<CharMonster>();
                default:
                    return null;
            }

            //NavMesh는 기획 결정에 따라 추가할 것.
        }

        // 캐릭터 내 필요한 하위 오브젝트 
        public static void AddDescendant(GameObject go)
        {
            GameObject Descendant = new GameObject("FightCollider");
            Descendant.transform.SetParent(go.transform, false);
            Descendant.AddComponent<CapsuleCollider>();

            Descendant = new GameObject("MoveCollider");
            Descendant.transform.SetParent(go.transform, false);
            Descendant.AddComponent<CapsuleCollider>();

            Descendant = new GameObject("SkillRoot");
            Descendant.transform.SetParent(go.transform, false);

            Descendant = new GameObject("CameraPos");
            Descendant.transform.SetParent(go.transform, false);
        }
    }
}
#endif