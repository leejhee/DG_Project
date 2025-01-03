using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public class TestManager : Singleton<TestManager>
    {
        public void TestDebug()
        {
            Debug.Log("It's Run");
        }

    }
}