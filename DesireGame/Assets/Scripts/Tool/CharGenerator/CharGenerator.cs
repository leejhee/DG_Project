#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    public class CharGenerator : EditorWindow
    {
        Object SPUM_Object = null;

        Dictionary<long, SheetData> CharDict;       
        string[] Options;
        int selectedOptionIndex;

        string targetPath = "Assets/Resources/Prefabs/Char/";

        Editor SPUMEditor;

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

        void OnDisable()
        {
            AssetDatabase.SaveAssets();
            DataManager.Instance.ClearCache();
        }

        void OnGUI()
        {
            GUILayout.Label("ĳ���� ���� ��", EditorStyles.boldLabel);

            #region Put SPUM Object
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SPUM ���� ĳ���͸� �־��ּ���.");
            GUILayout.FlexibleSpace();
            SPUM_Object = EditorGUILayout.ObjectField(SPUM_Object, typeof(GameObject), false, GUILayout.MaxWidth(300));
            EditorGUILayout.EndHorizontal();
            #endregion

            EditorGUILayout.HelpBox("SPUM ���� ĳ���Ϳ� ĳ���� �ʼ� ����� �����Ͽ� �ֽ��ϴ�.", MessageType.Info);

            #region Select Data for New Prefab
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("� ĳ������ �������ΰ���?");
            GUILayout.FlexibleSpace();
            selectedOptionIndex = EditorGUILayout.Popup(selectedOptionIndex, Options, GUILayout.MaxWidth(300));
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Select Path
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("������ �̸��� �����ϰ� �Է����ּ���.");
            GUILayout.FlexibleSpace();
            EditorGUILayout.TextField(targetPath, GUILayout.MaxWidth(500));
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Set Preview of Prefab
            if (SPUM_Object != null)
            {
                if (SPUMEditor == null || SPUMEditor.target != SPUM_Object)
                {
                    DestroyImmediate(SPUMEditor); // �ּ� ó���� �κ� �״�� ����
                    SPUMEditor = Editor.CreateEditor(SPUM_Object);
                }
                SPUMEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 100), GUIStyle.none);
            }
            #endregion


            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ĳ���� ���� �Ǵ� ��� �߰�", GUILayout.Width(300)))
            {
                GeneratorChar();
            }   
        }

        /// <summary>
        /// ���� ��� �߰��� �ʿ��� �� ���� �ݵ�� �̸� �����ϰ� �۾��� ��
        /// </summary>
        private void GeneratorChar()
        {
            long selectedId = new List<long>(CharDict.Keys)[selectedOptionIndex];
            CharData targetData = CharDict[selectedId] as CharData;

            if (SPUM_Object is null)
            {
                Debug.LogError("���� �����ϰ� �����̳���?");
                return;
            }

            #region COPY SELECTED SPUM ASSET
            string assetPath = AssetDatabase.GetAssetPath(SPUM_Object);
            string savePath = targetPath + targetData.charPrefab + ".prefab";
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("������ �ƴϷ���!");               
                return;
            }
            else if (File.Exists(savePath))
            {
                Debug.LogError($"�ߺ��Ǵ� �̸��� ������ ������ ������ �����մϴ�. {targetPath}�� Ȯ�����ּ���.");
                return;
            }
            #endregion

            #region LOAD COPIED ASSET AND EDIT

            GameObject CharPrefab = new GameObject(targetData.charPrefab);
            CharPrefab.transform.position = new Vector3(0, 1f, 0);

            #region TEMPORARY OBJECT - SPUMPrefab
            GameObject SPUMPrefab = PrefabUtility.InstantiatePrefab(SPUM_Object) as GameObject;
            if (!SPUMPrefab)
            {
                Debug.LogError("�����ϰ� �ε��ߴµ� null? ���� �߸���. ���⿡ �ɸ��� �ȵ�.");
                return;
            }
            PrefabUtility.UnpackPrefabInstance(SPUMPrefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            List<Transform> children = new List<Transform>();
            foreach(Transform t in SPUMPrefab.transform.GetComponentInChildren<Transform>())
                children.Add(t);           
            foreach (Transform child in children)
            {
                child.SetParent(CharPrefab.transform);                
                Quaternion rotY = Quaternion.Euler(0f, 90f, 0f);
                Quaternion rotX = Quaternion.Euler(270f, 0f, 0f);
                child.SetPositionAndRotation(new Vector3(0, 1f, 0), rotY * rotX);
            }

            DestroyImmediate(SPUMPrefab);
            #endregion

            CharBase newCharBase = CharFactory.AddBaseComponent(targetData, CharPrefab);
            SerializedObject serialized = new SerializedObject(newCharBase);

            SerializedProperty Index = serialized.FindProperty("_index");
            Index.longValue = targetData.Index;
            serialized.ApplyModifiedProperties();

            CharFactory.CharacterizeBase(CharPrefab);
            CharPrefab.name = targetData.charPrefab;

            PrefabUtility.SaveAsPrefabAsset(CharPrefab, savePath);
            #endregion
            Debug.Log($"���������� {CharPrefab.name} �����߽��ϴ�");
            DestroyImmediate(CharPrefab);            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

    }

    /// <summary> �����Ϳ��� CharBase ����ϴ� ĳ���� �̴� �� </summary>
    public static class CharFactory
    {
        public static CharBase AddBaseComponent(CharData data, GameObject go)
        {
            var nav = go.AddComponent<NavMeshAgent>();
            nav.baseOffset = 0.5f;
            switch (data.charType)
            {
                case SystemEnum.eCharType.NEUTRAL:
                    return go.AddComponent<CharNPC>();
                case SystemEnum.eCharType.ALLY:
                    return go.AddComponent<CharPlayer>();
                case SystemEnum.eCharType.ENEMY:
                    return go.AddComponent<CharMonster>();
                default:
                    return null;
            }

            //NavMesh�� ��ȹ ������ ���� �߰��� ��.
        }

        // ĳ���� �� �ʿ��� ���� ������Ʈ �� ������Ʈ ����
        public static void CharacterizeBase(GameObject go)
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