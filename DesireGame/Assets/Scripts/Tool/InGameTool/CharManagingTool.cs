#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client
{
    // �̰� �� ������°� : 
    // ���̾��Ű���� Delete�� ĳ���͸� ���ָ� NRE�� ��â ��ű� ����.
    // ������ �׽����� ���� ���� �������� ĳ���͸� ���ִ� �����
    public class CharManagingTool : EditorWindow
    {
        private List<CharBase> characterList = new();
        private int VictimOrder = -1;

        Vector2 scrollPos;

        [MenuItem("DG_InGame/ĳ���� ���� �� ����")]
        public static void ShowWindow()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity �÷��� �� ��� �ٶ��ϴ�.");
                return;
            }
            GetWindow(typeof(CharManagingTool), false, "Character Managing Tool");

        }

        private void OnEnable()
        {
            if (EditorApplication.isPlaying)
            {
                UpdateCharacterList();
            }
        }

        private void UpdateCharacterList() => characterList = CharManager.Instance.GetCurrentCharacters();

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity �÷��� �� ��� �ٶ��ϴ�.");
                Close();
            }
                        
            EditorGUILayout.HelpBox("������ �� ����� �����մϴ�.", MessageType.Warning);
            
            EditorGUILayout.Space();

            string[] characterNames = characterList.Select(c => $"{c.GetID()} - {c.CharData.charName}").ToArray();

            EditorGUILayout.LabelField("������ ĳ���� ����", EditorStyles.boldLabel);
            VictimOrder = EditorGUILayout.Popup("ĳ���� ����", VictimOrder, characterNames);

            var victim = VictimOrder == -1 ? null : characterList[VictimOrder];
            string guide = victim == null ? "������ ĳ���͸� �������ּ���." : $"{victim.GetID()}�� ĳ���� {victim.name}�� �����մϴ�.";

            GUIStyle ButtonStyle = new(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleCenter
            };

            if (GUILayout.Button(guide, ButtonStyle, GUILayout.ExpandWidth(true)) &&
                (victim == true)&& VictimOrder > -1)
            {
                victim.Dead.Invoke();
                UpdateCharacterList();
            }

        }

    }
}

#endif