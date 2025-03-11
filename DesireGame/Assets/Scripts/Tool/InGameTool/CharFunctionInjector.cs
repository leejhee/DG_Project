#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.PackageManager.UI;

namespace Client
{
    public class CharFunctionInjector : EditorWindow
    {
        private List<CharBase> characterList = new();
        private List<FunctionData> functionDatas = new();
        private string[] functionOptions = null;
        private string searchFunctionTerm = "";
        private int CasterOrder = -1;
        private int TargetOrder = -1;
        private FunctionData selectedFunction = null;

        

        Vector2 scrollPos;

        [MenuItem("Tools/Function Injector")]
        public static void ShowWindow()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity 플레이 후 사용 바랍니다.");
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
                functionOptions = new string[functionlist.Count];
                for (int i = 0; i < functionlist.Count; i++)
                {
                    var target = functionlist[i] as FunctionData;
                    functionDatas.Add(target);
                    functionOptions[i] =
                        $"Index : {target.Index}, " +
                        $"Function Type : {target.function}, " +
                        $"Duration : {(target.time <= 1 ? target.time : target.time / SystemConst.PER_THOUSAND)}";
                }              
            }            
        }

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity 플레이 후 사용 바랍니다.");
                Close();
            }

            string[] characterNames = characterList.Select(c => $"{c.GetID()} - {c.CharData.charName}").ToArray();

            EditorGUILayout.LabelField("Function을 주는 캐릭터 선택", EditorStyles.boldLabel);
            CasterOrder = EditorGUILayout.Popup("캐릭터 선택", CasterOrder, characterNames);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Function을 받는 캐릭터 선택", EditorStyles.boldLabel);
            TargetOrder = EditorGUILayout.Popup("캐릭터 선택", TargetOrder, characterNames);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("주입할 Function 선택"), EditorStyles.boldLabel);
            searchFunctionTerm = EditorGUILayout.TextField("Search Field", searchFunctionTerm);

            EditorGUILayout.Space();

            var filteredFunctions = functionDatas
            .Where(f => string.IsNullOrEmpty(searchFunctionTerm) ||
                        f.function.ToString().Contains(searchFunctionTerm) ||
                        f.Index.ToString().Contains(searchFunctionTerm) ||
                        f.time.ToString().Contains(searchFunctionTerm))
            .ToList();


            // 검색 결과 출력 (선택 가능)
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
                    EditorGUILayout.LabelField(function.time.ToString(), rowStyle, GUILayout.Width(80));

                    if (GUILayout.Button("선택", GUILayout.Width(80)))
                    {
                        selectedFunction = function;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("검색 결과가 없습니다.", MessageType.Warning);
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("버프 적용", new GUIStyle(EditorStyles.boldLabel), GUILayout.Height(30)) && 
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
                Debug.LogWarning($"{caster.GetID()}번 캐릭터에서 {target.GetID()}번 캐릭터에 {selectedFunction.Index}번 function 주입");
            }
        }

        
    }
}

#endif