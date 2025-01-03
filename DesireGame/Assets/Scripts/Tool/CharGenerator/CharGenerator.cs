#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Client
{
    public class CharGenerator : EditorWindow
    {
        Object SPUM_Object = null;

        [MenuItem("Tools/CharGenerator")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CharGenerator), false, "My Editor");
        }
        void OnGUI()
        {

            GUILayout.Label("캐릭터 생성 툴", EditorStyles.boldLabel);
            SPUM_Object = EditorGUILayout.ObjectField("SPUM 원형 캐릭터를 넣어주세요.", SPUM_Object, typeof(GameObject), false);
            EditorGUILayout.HelpBox("SPUM 원형 캐릭터에 캐릭터 필수 기능을 제작하여 넣습니다.", MessageType.Info);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("캐릭터 생성 또는 기능 추가",GUILayout.Width(100)))
            {
                GeneratorChar();
            }
        }

        /// <summary>
        /// 이후 기능 추가가 필요할 수 있음 반드시 이를 생각하고 작업할 것
        /// </summary>
        private void GeneratorChar()
        { 
            


        }
    }
}
#endif