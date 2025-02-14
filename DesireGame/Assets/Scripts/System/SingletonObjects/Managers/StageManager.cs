using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class StageManager : Singleton<StageManager>
    {
        private StageManager() { }
        
        // 현재 스테이지
        public int Stage { get; private set; }
        // 스테이지 시작 가능 상태
        public bool CanStartStage { get; private set; } = true;
        
        public void StartStage(int stageNum)
        {
            if (CanStartStage == false)
                return;

            if (DataManager.Instance.MonsterSpawnStageMap.ContainsKey(stageNum) == false)
                return;
            var StageMonsterList = DataManager.Instance.MonsterSpawnStageMap[stageNum];

            foreach (var monsterInfo in StageMonsterList)
            {
                CharBase charMoster = CharManager.Instance.CharGenerate(monsterInfo.MonsterID);
                var tileObj = TileManager.Instance.GetTile(monsterInfo.PositionIndex);
            }
        }

    }
}