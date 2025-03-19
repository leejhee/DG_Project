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
            GUILayout.Label("적을 선택하면 \n그 적이 드랍하는 아이템들을 볼 수 있습니다.", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enemy ID : ");
            enemyID = EditorGUILayout.LongField(enemyID);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (EditorApplication.isPlaying == false)
            {
                EditorGUILayout.HelpBox($"Unity 플레이 후 사용 바랍니다.", MessageType.Info);
                return;
            }

            StringBuilder sb = new();
            List<EnemyData> enemyDatas = new();
            var EnemyDataList = DataManager.Instance.GetDataList<EnemyData>();
            if (EnemyDataList == null)
            {
                Debug.LogWarning("CharTool : CharDataList 를 찾지 못함");
            }
            foreach (var _enemyData in EnemyDataList)
            {
                var enemyData = _enemyData as EnemyData;
                if (enemyData == null)
                    continue;

                sb.AppendLine($"{enemyData.Index} {enemyData.enemyName}");
            }

            EditorGUILayout.HelpBox($"사용 가능 적 명단 \n{sb.ToString()}", MessageType.Info);
            GUILayout.FlexibleSpace();


            if (GUILayout.Button("아이템 드랍", GUILayout.Width(300)))
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
            Debug.Log($"{enemyID}가 드랍할 건 {dropList.Count}개, {sb.ToString()}");

        }

    }
}
#endif