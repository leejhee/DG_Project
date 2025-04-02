using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{

    /// <summary>
    /// const System
    /// </summary>
    public class SystemConst 
    {
        public static int FPS = 60;

        public static float TILE_UNIT_LENGTH = 1f;

        // 맵 타일 구조가 불변이라는 전제
        public static int TILE_COL_OFFSET = 5;  // 한 column에 몇개? (다 column 단위로 기획되어 있어서 이렇게 함)
        public static int TILE_COL_COUNT = 4;   // column이 몇개?
        public static int TILE_SIDE_OFFSET = TILE_COL_OFFSET * TILE_COL_COUNT;
        public static int TILE_MAX = TILE_SIDE_OFFSET * 2; // 전체 타일 수

        public static int DEFAULT_MANA_RESTORE = 5;

        public static long NO_CONTENT = 0;              // 빈 인덱스용
        public static long SYNERGY_TRIGGER = 600001;    // 시너지 트리거용

        public static float PER_TEN_THOUSAND = 10000.0f;
        public static float PER_THOUSAND = 1000.0f;
        public static float PER_CENT = 100.0f;
    }

    /// <summary>
    /// const System String
    /// </summary>
    public class SystemString
    {
        public const string AudioSystem = "AudioSystem";
        public const string PlayerHitCollider = "PlayerHitCollider";
        public const string MonsterHitCollider = "MonsterHitCollider";
    }


}