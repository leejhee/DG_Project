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