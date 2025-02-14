using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class StageManager : Singleton<StageManager>
    {
        private StageManager() { }
        
        // ���� ��������
        public int Stage { get; private set; }
        // �������� ���� ���� ����
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