using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;


#if UNITY_EDITOR
using UnityEditor;

public class Test : Editor
{
    [MenuItem("Test/Test1")]
    public static void Test1()
    {
        GameSceneMessageParam tmp = new();
        tmp.message = "public class Test:Editor ���� ��ε�ĳ������ �޽����Դϴ�.";
        MessageManager.SendMessage<GameSceneMessageParam>(tmp);
    }

    [MenuItem("Test/Test2")]
    public static void Test2()
    {

    }

    [MenuItem("Test/Test3")]
    public static void Test3()
    {

    }
}
#endif
