using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Client
{
    public class StageManager : Singleton<StageManager>
    {
        private StageManager() 
        {
            
        }
        
        // 현재 스테이지
        public int Stage { get; private set; }
        // 스테이지 시작 가능 상태
        public bool CanStartStage { get; private set; } = true;

        public override void Init()
        {
            base.Init();
            CharManager.Instance.OnCharTypeEmpty += CheckWinCondition;
            Stage = 0;
        }

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

        /// <summary>
        /// 이걸 호출할 때마다 다음 스테이지로 갈고예요 라는 뜻
        /// </summary>
        public void MoveToNextStage()
        {
            if(TryGetNextStage(Stage))
            {
                StartStage(++Stage);
            }
            else
            {
                Debug.LogError("다음 스테이지 없음");
            }
        }

        /// <summary>
        /// 승패 판정
        /// </summary>
        public void CheckWinCondition(Type charType)
        {
            Debug.Log($"{charType} 타입의 모든 캐릭터가 제거되었습니다.");

            if (charType == typeof(CharPlayer))
            {
                Debug.Log("모든 플레이어가 죽었습니다. 졌당..");
                // 게임 오버 처리
            }
            else if (charType == typeof(CharMonster))
            {
                Debug.Log("모든 적이 사라졌습니다. 이겼당!!");
                MoveToNextStage();
            }
        }

        /// <summary>
        /// 다음 스테이지 정보 가져오기
        /// </summary>
        public bool TryGetNextStage(int _stage)
        {
            CharManager.Instance.ClearAllChar();

            int nextStage = _stage + 1;
            if (DataManager.Instance.MonsterSpawnStageMap.ContainsKey(nextStage) == false)
            {
                return false;
            }
            var stageList = DataManager.Instance.MonsterSpawnStageMap[nextStage];
            foreach (var stage in stageList)
            {
                CharBase charMonster = CharManager.Instance.CharGenerate(stage.MonsterID);
                TileManager.Instance.SetChar(stage.PositionIndex, charMonster);
            }
            return true;

        }
    }
}