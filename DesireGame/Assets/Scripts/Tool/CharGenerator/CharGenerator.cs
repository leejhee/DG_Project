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

            GUILayout.Label("ĳ���� ���� ��", EditorStyles.boldLabel);
            SPUM_Object = EditorGUILayout.ObjectField("SPUM ���� ĳ���͸� �־��ּ���.", SPUM_Object, typeof(GameObject), false);
            EditorGUILayout.HelpBox("SPUM ���� ĳ���Ϳ� ĳ���� �ʼ� ����� �����Ͽ� �ֽ��ϴ�.", MessageType.Info);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ĳ���� ���� �Ǵ� ��� �߰�",GUILayout.Width(100)))
            {
                GeneratorChar();
            }
        }

        /// <summary>
        /// ���� ��� �߰��� �ʿ��� �� ���� �ݵ�� �̸� �����ϰ� �۾��� ��
        /// </summary>
        private void GeneratorChar()
        { 
            


        }
    }
}
#endif