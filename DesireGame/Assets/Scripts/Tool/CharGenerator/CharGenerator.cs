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
            GUILayout.Label("ĳ���� ���� ��", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SPUM ���� ĳ���͸� �־��ּ���.");
            GUILayout.FlexibleSpace();
            SPUM_Object = EditorGUILayout.ObjectField(SPUM_Object, typeof(GameObject), false, GUILayout.MaxWidth(300));
            EditorGUILayout.EndHorizontal();
           
            EditorGUILayout.HelpBox("SPUM ���� ĳ���Ϳ� ĳ���� �ʼ� ����� �����Ͽ� �ֽ��ϴ�.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("� ĳ������ �������ΰ���?");
            GUILayout.FlexibleSpace();
            selectedOptionIndex = EditorGUILayout.Popup(selectedOptionIndex, Options, GUILayout.MaxWidth(300));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("������ �̸��� �����ϰ� �Է����ּ���.");
            GUILayout.FlexibleSpace();
            EditorGUILayout.TextField(targetPath, GUILayout.MaxWidth(500));
            EditorGUILayout.EndHorizontal();

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

            GameObject SPUMPrefab = PrefabUtility.InstantiatePrefab(PrefabUtility.SaveAsPrefabAsset(SPUM_Object as GameObject, savePath)) as GameObject;
            if (!SPUMPrefab)
            {
                Debug.LogError("�����ϰ� �ε��ߴµ� null? ���� �߸���. ���⿡ �ɸ��� �ȵ�.");
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
            // ������ ���� �̸� �ٲٱ�
            SPUMPrefab.name = targetData.charPrefab;
            string renameResult = AssetDatabase.RenameAsset(savePath, targetData.charPrefab);
            if (string.IsNullOrEmpty(renameResult))
                Debug.Log($"���������� �������� {SPUMPrefab.name}���� ��ϵǾ����ϴ�! ���� ��ο��� Ȯ�����ּ���.");
            else
                Debug.LogError($"{renameResult}�� �����Դϴ�. {targetPath}�� ���� �̸��� ������ ������ Ȯ���ϼ���.");

            AssetDatabase.SaveAssets();
            #endregion
        }

    }

    /// <summary> �����Ϳ��� CharBase ����ϴ� ĳ���� �̴� �� </summary>
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

            //NavMesh�� ��ȹ ������ ���� �߰��� ��.
        }

        // ĳ���� �� �ʿ��� ���� ������Ʈ 
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