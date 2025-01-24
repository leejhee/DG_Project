using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    public class TestManager : Singleton<TestManager>
    {
        public GlobalSave LobbySave;
        public GameSave currentSave;

        TestManager() { }

        public void TestDebug()
        {
            Debug.Log("It's Run");
        }

        public void TestSave()
        {
            LobbySave = new GlobalSave();
            currentSave = new GameSave();
            LobbySave.IngameSaves.Add(currentSave);
        }

    }
}