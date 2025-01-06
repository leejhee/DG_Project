using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    /// <summary>
    /// 게임에 필요한 Enum
    /// </summary>
    public class SystemEnum
    {
        public enum eSounds
        {
            BGM,
            SFX,
            MaxCount
        }
        public enum eBGM
        {

            MaxCount
        }
        public enum eSFX
        {
            MaxCount
        }
        
        public enum eTag
        {
            MaxCount
        }

        /// <summary>
        /// UI Event 종류 지정
        /// </summary>
        public enum eUIEvent
        {
            Click,
            Drag,
            DragEnd,

            MaxCount
        }

        public enum eScene
        {
            Title,
            Loby,
            GameScene,

            MaxCount
        }
        
        public enum EGameData
        {
            GAME1,
            GAME2,
                        
            EMax
        }

        public enum ERound
        {
            ROUND1,
            ROUND2,
            ROUND3,

            EMax
        }

        public enum EKeywordType
        {
            STRATEGY_KEY,
            CHARACTER_KEY,

            EMax
        }

        public enum ECharImg
        {
            DEFAULT,
            SMILE,
            SOB,
            RAGE,
            WORRY,
            SHOCK,

            EMax
        }

        public enum EDialougeEff
        {
            None,
        }
    }
}