#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class ItemTool : EditorWindow
    {
        private long enemyID = 210000;

        [MenuItem("DG_InGame/Drop Item")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ItemTool), false, "Drop Item");
        }

        void OnGUI()
        {
            GUILayout.Label("���� �����ϸ� \n�� ���� ����ϴ� �����۵��� �� �� �ֽ��ϴ�.", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enemy ID : ");
            enemyID = EditorGUILayout.LongField(enemyID);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (EditorApplication.isPlaying == false)
            {
                EditorGUILayout.HelpBox($"Unity �÷��� �� ��� �ٶ��ϴ�.", MessageType.Info);
                return;
            }

            StringBuilder sb = new();
            List<EnemyData> enemyDatas = new();
            var EnemyDataList = DataManager.Instance.GetDataList<EnemyData>();
            if (EnemyDataList == null)
            {
                Debug.LogWarning("CharTool : CharDataList �� ã�� ����");
            }
            foreach (var _enemyData in EnemyDataList)
            {
                var enemyData = _enemyData as EnemyData;
                if (enemyData == null)
                    continue;

                sb.AppendLine($"{enemyData.Index} {enemyData.enemyName}");
            }

            EditorGUILayout.HelpBox($"��� ���� �� ��� \n{sb.ToString()}", MessageType.Info);
            GUILayout.FlexibleSpace();


            if (GUILayout.Button("������ ���", GUILayout.Width(300)))
            {
                DropItems();
            }

        }

        void DropItems()
        {
            List<long> dropList = ItemManager.Instance.SetDropTableList(enemyID);

            StringBuilder sb = new();
            foreach (long id in dropList)
            {
                sb.Append($"{id} ");
            }
            Debug.Log($"{enemyID}�� ����� �� {dropList.Count}��, {sb.ToString()}");

        }

    }
}
#endif