using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
namespace Client
{
    public class DataTool
    {
        [MenuItem("Data/�����Ͱ���")]
        public static void DataVerification()
        {
            DataManager.Instance.DataLoad();
        }
    }
}
#endif
