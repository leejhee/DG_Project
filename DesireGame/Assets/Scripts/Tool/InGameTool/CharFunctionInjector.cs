#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace Client
{
    public class CharFunctionInjector : EditorWindow
    {
        private List<CharBase> characterList = new();
        private List<FunctionData> functionDatas = new();
        private string searchFunctionTerm = "";
        private int CasterOrder = -1;
        private int TargetOrder = -1;
        private FunctionData selectedFunction = null;

        

        Vector2 scrollPos;

        [MenuItem("DG_InGame/ĳ���Ϳ� Function ����")]
        public static void ShowWindow()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity �÷��� �� ��� �ٶ��ϴ�.");
                return;
            }
            EditorWindow.GetWindow(typeof(CharFunctionInjector), false, "Character Function Injector");

        }

        private void OnEnable()
        {
            if (EditorApplication.isPlaying)
            {
                characterList = CharManager.Instance.GetCurrentCharacters();
                var functionlist = DataManager.Instance.GetDataList<FunctionData>();
                for (int i = 0; i < functionlist.Count; i++)
                {
                    var target = functionlist[i] as FunctionData;
                    functionDatas.Add(target);
                }              
            }            
        }

        private string TimeGuide(long duration)
        {
            if(duration == default)
            {
                return "INSTANT";
            }
            else if (duration == -1)
            {
                return "PERMANENT";
            }
            else
            {
                return ((float)(duration / SystemConst.PER_THOUSAND)).ToString();
            }
        }

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity �÷��� �� ��� �ٶ��ϴ�.");
                Close();
            }

            string[] characterNames = characterList.Select(c => $"{c.GetID()} - {c.CharData.charName}").ToArray();

            EditorGUILayout.LabelField("Function�� �ִ� ĳ���� ����", EditorStyles.boldLabel);
            CasterOrder = EditorGUILayout.Popup("ĳ���� ����", CasterOrder, characterNames);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Function�� �޴� ĳ���� ����", EditorStyles.boldLabel);
            TargetOrder = EditorGUILayout.Popup("ĳ���� ����", TargetOrder, characterNames);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("������ Function ����"), EditorStyles.boldLabel);
            searchFunctionTerm = EditorGUILayout.TextField("Search Field", searchFunctionTerm);

            EditorGUILayout.Space();

            var filteredFunctions = functionDatas
            .Where(f => string.IsNullOrEmpty(searchFunctionTerm) ||
                        f.function.ToString().Contains(searchFunctionTerm) ||
                        f.Index.ToString().Contains(searchFunctionTerm) ||
                        TimeGuide(f.time).Contains(searchFunctionTerm))
            .ToList();


            // �˻� ��� ��� (���� ����)
            if (filteredFunctions.Count > 0)
            {
                GUIStyle headerStyle = new(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };

                GUIStyle rowStyle = new(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft
                };

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Index", headerStyle, GUILayout.Width(80));
                EditorGUILayout.LabelField("Function Type", headerStyle, GUILayout.Width(200));
                EditorGUILayout.LabelField("Duration", headerStyle, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                for (int i = 0; i < filteredFunctions.Count; i++)
                {
                    FunctionData function = filteredFunctions[i];

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);

                    EditorGUILayout.LabelField(function.Index.ToString(), rowStyle, GUILayout.Width(80));
                    EditorGUILayout.LabelField(function.function.ToString(), rowStyle, GUILayout.Width(200));
                    EditorGUILayout.LabelField(TimeGuide(function.time), rowStyle, GUILayout.Width(80));

                    if (GUILayout.Button("����", GUILayout.Width(80)))
                    {
                        selectedFunction = function;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("�˻� ����� �����ϴ�.", MessageType.Warning);
            }

            GUILayout.FlexibleSpace();

            string result = selectedFunction == null ? "������ �������ּ���" : selectedFunction.Index.ToString() + "�� ���� ����";

            GUIStyle ButtonStyle = new(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30, 
                alignment = TextAnchor.MiddleCenter
            };

            if (GUILayout.Button(result, ButtonStyle, GUILayout.ExpandWidth(true)) && 
                selectedFunction != null && CasterOrder > -1 && TargetOrder > -1)
            {
                var caster = characterList[CasterOrder];
                var target = characterList[TargetOrder];
                target.FunctionInfo.AddFunction(new BuffParameter()
                {
                    CastChar = caster,
                    TargetChar = target,
                    FunctionIndex = selectedFunction.Index,
                    eFunctionType = selectedFunction.function
                });
                Debug.LogWarning($"{caster.GetID()}�� ĳ���Ϳ��� {target.GetID()}�� ĳ���Ϳ� {selectedFunction.Index}�� function ����");
            }
        }

        
    }
}

#endif