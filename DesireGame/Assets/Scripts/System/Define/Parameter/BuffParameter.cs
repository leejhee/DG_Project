using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    // �⺻ ���� ���� ������ 
    public struct BuffParameter
    {
        public eFunction eFunctionType;
        public CharBase TargetChar;
        public CharBase CastChar;
        public long FunctionIndex;
    }
}