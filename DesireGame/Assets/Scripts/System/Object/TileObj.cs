using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class TileObj : MonoBehaviour
    {
        [SerializeField] private int _tileIndex;
        [SerializeField] private eCharType _teamTile = eCharType.None;
        [SerializeField] private TileCombatBehaviour _tileCombat;

        private CharBase _charBase = null;

        public int TileIndex => _tileIndex;
        public eCharType TeamTile => _teamTile;

        #region 전투 컴포넌트 기능 연결
        public void SwitchCombatBehaviour(bool iscombat) => _tileCombat.enabled = iscombat;
        public bool Accessible => _tileCombat.IsEmpty;
        public HashSet<CharBase> GetTotalSteppingChar(HashSet<CharBase> other) { return _tileCombat.GetTotalChar(other); }
        #endregion

        private void Start()
        {
            TileManager.Instance.SetTile(_tileIndex, this);
        }

        // 캐릭터 세팅
        public CharBase SetChar(CharBase charBase)
        {
            if (charBase == false)
            {
                _charBase = null;
                return null;
            }
            _charBase = charBase;
            _charBase.TileIndex = _tileIndex;
            _charBase.transform.position = this.transform.position;

            return _charBase;
        }

        public CharBase GetChar()
        {
            return _charBase;
        }
        public bool IsCharSet()
        {
            return _charBase == true;
        }
        public void ClearTile()
        {
            _charBase = null;
        }

    }
}
