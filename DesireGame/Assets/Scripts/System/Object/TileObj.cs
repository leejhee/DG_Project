using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class TileObj : MonoBehaviour
    {
        [SerializeField] private int _tileIndex;
        private CharBase _charBase = null;

        public int TileIndex => _tileIndex;
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

            _charBase.transform.position = this.transform.position;

            return _charBase;
        }

        public CharBase GetChar()
        {
            return _charBase;
        }

        public void ClearTile()
        {
            _charBase = null;
        }

    }
}
