#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client
{
    /// <summary> 역할 : 전투 테스트 </summary>
    public class BattleTestTool : EditorWindow
    {
        private enum TabType { Add, Delete }
        
        private TabType _tabType = TabType.Add;
        private List<CharBase> _characterList = new();
        private List<CharData> _charDataList = new();
        private string[] _dataOptions; 
            
        private int _addOrder = -1;
        private int _victimOrder = -1;

        private int _tile = 0;
        
        Vector2 scrollPos;

        [MenuItem("DG_InGame/스킬 사용 테스트 벤치")]
        public static void ShowWindow()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity 플레이 후 사용 바랍니다.");
                return;
            }
            EditorWindow window = GetWindow(typeof(BattleTestTool), false, "Character Managing Tool");
            window.minSize = new Vector2(400, 300);
        }

        private void OnEnable()
        {
            if (EditorApplication.isPlaying)
            {
                UpdateCharacterList();
                var dataList = DataManager.Instance.GetDataList<CharData>();
                foreach (var data in dataList)
                {
                    _charDataList.Add(data as CharData);
                }
                _dataOptions = _charDataList.Select(data => $"{data.charName}\t{data.charType}\t{data.Index}").ToArray();
            }
        }

        private void UpdateCharacterList()
        {
            _characterList = CharManager.Instance.GetCurrentCharacters();
            Repaint();
        }

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Unity 플레이 후 사용 바랍니다.");
                Close();
            }
                        
            EditorGUILayout.HelpBox("비전투 시 사용을 권장합니다.", MessageType.Warning);
            
            EditorGUILayout.Space();
            
            #region Custom your Character Pool
            _tabType = (TabType)GUILayout.Toolbar((int)_tabType, new[] { "캐릭터 추가", "캐릭터 삭제" });
            switch (_tabType)
            {
                case TabType.Add:
                    DrawAddTab();
                    break;
                case TabType.Delete:
                    DrawDeleteTab();
                    break;
            }
            #endregion
            
        }

        private void DrawAddTab()
        {
            #region 캐릭터 선택
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("추가할 캐릭터 선택");
            GUILayout.FlexibleSpace();
            _addOrder = EditorGUILayout.Popup(_addOrder, _dataOptions);
            EditorGUILayout.EndHorizontal();
            
            CharData data = _addOrder == -1 ? null : _charDataList[_addOrder];
            if (data == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("추가할 캐릭터를 선택해주세요.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
                return;
            }
            #endregion
            
            #region 타일 정보 입력
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("타일 인덱스 : ");
            GUILayout.FlexibleSpace();
            _tile = EditorGUILayout.IntField(_tile);
            EditorGUILayout.EndHorizontal();
            #endregion
            
            #region 타일 유효성 검사
            var tileObj = TileManager.Instance.GetTile(_tile);
            if (tileObj.IsCharSet())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"{_tile} 에는 이미 캐릭터가 올라가있습니다.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
                return;
            }

            if (!SystemConst.IsRightSide(_tile, data.charType))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"{_tile}과 {data.charType}이 상충되므로 타일 인덱스를 수정하세요.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
                return;
            }
            #endregion
            
            GUIStyle buttonStyle = new(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleCenter
            };
            
            if (GUILayout.Button("선택한 캐릭터 추가", buttonStyle, GUILayout.ExpandWidth(true)) && _addOrder > -1)
            {
                CharManager.Instance.CharGenerate(
                    new CharTileParameter(SystemEnum.eScene.GameScene, _tile, data.Index));
                UpdateCharacterList();
            }
        }

        private void DrawDeleteTab()
        {
            string[] characterNames = _characterList.Select(c => $"{c.GetID()} - {c.CharData.charName}").ToArray();

            EditorGUILayout.LabelField("삭제할 캐릭터 선택", EditorStyles.boldLabel);
            _victimOrder = EditorGUILayout.Popup("캐릭터 선택", _victimOrder, characterNames);

            var victim = _victimOrder == -1 ? null : _characterList[_victimOrder];
            string guide = !victim ? "삭제할 캐릭터를 선택해주세요." : $"{victim.GetID()}번 캐릭터 {victim.name}을 삭제합니다.";
            
            GUIStyle buttonStyle = new(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleCenter
            };

            if (GUILayout.Button(guide, buttonStyle, GUILayout.ExpandWidth(true)) &&
                victim == true && _victimOrder > -1)
            {
                victim.Dead();
                UpdateCharacterList();
            }
        }
    }
}

#endif