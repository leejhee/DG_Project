using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public struct CharParameter
    {
        public SystemEnum.eScene Scene;
        public Vector3 GeneratePos;
        public long CharIndex;

        public CharParameter(SystemEnum.eScene scene, Vector3 Pos, long index)
        {
            Scene = scene;
            GeneratePos = Pos;
            CharIndex = index;
        }
    }
}