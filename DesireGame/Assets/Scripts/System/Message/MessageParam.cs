using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    #region Map
    public class GameSceneMessageParam : MessageSystemParam
    {
        public string message;
    }
    #endregion

    #region CharBase

    public class PlayerMove : MessageSystemParam
    {
        public int beforeTileIndex = default;
        public CharBase moveChar = default;
    }

    #endregion
}