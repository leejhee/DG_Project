#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Client
{
    public class StageTool : EditorWindow
    {
        private int StageNum = 1;
        [MenuItem("DG_InGame/�������� ���� ��ġ")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(StageTool), false, "�������� ���� ��ġ");
        }

        void OnGUI()
        {
            GUILayout.Label("�������� Num", EditorStyles.boldLabel);

            #region �������� ���� �Է�
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�������� Num: ");
            GUILayout.FlexibleSpace();
            StageNum = EditorGUILayout.IntField(StageNum);
            EditorGUILayout.EndHorizontal();
            #endregion

            if (EditorApplication.isPlaying == false)
            {
                EditorGUILayout.HelpBox($"Unity�÷��� �� ��� �ٶ��ϴ�.", MessageType.Info);
                return;
            }

            #region �������� ���� ����
            StringBuilder stringBuilder = new();
            List<StageData> stageDatas = new();
            List<int> stageList = new();
            var stageDataKeyList = DataManager.Instance.MonsterSpawnStageMap?.Keys;
            if (stageDataKeyList == null)
            {
                Debug.LogWarning("StageTool : StageDataList �� ã�� ����");
                return;
            }
            foreach (var stageKey in stageDataKeyList)
            {
                stageList.Add(stageKey);
            }
            stringBuilder.Append($"��� ���� ���������� \n");
            foreach (var _stage in stageList)
            {
                stringBuilder.Append($"num : {_stage} \n");
            }
            stringBuilder.Append($"�Դϴ�. \n");

            EditorGUILayout.HelpBox($"��� ���� ĳ���� ��� \n {stringBuilder.ToString()}", MessageType.Info);
            if (stageList.Contains(StageNum) == false)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"{StageNum}�� ��� �Ұ����� ���������Դϴ�.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("�������� ����", GUILayout.Width(300)))
            {
                SetStage();
            }
        }

        private void SetStage()
        {
            CharManager.Instance.ClearAllChar();

            if (DataManager.Instance.MonsterSpawnStageMap.ContainsKey(StageNum) == false)
            {
                return;
            }
            var stageList =  DataManager.Instance.MonsterSpawnStageMap[StageNum];
            foreach(var stage in stageList)
            {
                CharBase charMonster = CharManager.Instance.CharGenerate(stage.MonsterID);
                TileManager.Instance.SetChar(stage.PositionIndex, charMonster);
            }
        }
    }
}

#endif