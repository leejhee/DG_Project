#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Client
{
    public class CharTool : EditorWindow
    {
        private long charIndex = -1;
        private int tile = 0;
        [MenuItem("DG_InGame/�ʿ� ĳ���� ��ġ")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CharTool), false, "�ʿ� ĳ���� ��ġ");
        }

        void OnGUI()
        {
            GUILayout.Label("���� ĳ���� �ε���", EditorStyles.boldLabel);

            #region ĳ���� ���� �Է�
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ĳ���� �ε��� : ");
            GUILayout.FlexibleSpace();
            charIndex = EditorGUILayout.LongField(charIndex);
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Ÿ�� ���� �Է�
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ÿ�� �ε��� : ");
            GUILayout.FlexibleSpace();
            tile = EditorGUILayout.IntField(tile);
            EditorGUILayout.EndHorizontal();
            #endregion
            if (EditorApplication.isPlaying == false)
            {
                EditorGUILayout.HelpBox($"Unity�÷��� �� ��� �ٶ��ϴ�.", MessageType.Info);
                return;
            }
            #region ĳ���� ���� ����
            StringBuilder stringBuilder = new();
            List<CharData> charDatas = new();
            var CharDataList =  DataManager.Instance.GetDataList<CharData>();
            if (CharDataList == null)
            {
                Debug.LogWarning("CharTool : CharDataList �� ã�� ����");
            }
            foreach (var _charData in CharDataList)
            {
                var charData = _charData as CharData;
                if (charData == null)
                    continue;

                stringBuilder.Append($" {charData.Index} �� ĳ���� ���� {charData.charName} �Դϴ�. \n");
            }

            EditorGUILayout.HelpBox($"��� ���� ĳ���� ��� \n {stringBuilder.ToString()}", MessageType.Info);
            var tileObj = TileManager.Instance.GetTile(tile);
            if (tileObj.IsCharSet())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"{tile} ���� �̹� ĳ���Ͱ� �ö��ֽ��ϴ�.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ĳ���� �ø���", GUILayout.Width(300)))
            {
                SetChar();
            }
        }

        private void SetChar()
        {
            CharBase charBase = CharManager.Instance.CharGenerate(charIndex);
            if (charBase == false)
                return;

            var tileObj = TileManager.Instance.GetTile(tile);
            // Ÿ���� null�̰ų� �̹� ĳ���Ͱ� ���õǾ�������
            if (tileObj?.IsCharSet() ?? true)
            {
                return;
            }
            TileManager.Instance.SetChar(tile, charBase);
        }
    }
}

#endif