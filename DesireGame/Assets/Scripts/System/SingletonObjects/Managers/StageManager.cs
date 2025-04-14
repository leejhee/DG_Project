using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Client
{
    public class StageManager : Singleton<StageManager>
    {
        private StageManager() { }
        
        // 현재 스테이지
        public int Stage { get; private set; }
        // 스테이지 시작 가능 상태
        public bool CanStartStage { get; private set; } = true;
        public bool IsStageFinished { get; private set; } = false;

        public Action OnStartStage;

        public override void Init()
        {
            base.Init();
            CharManager.Instance.OnCharTypeEmpty += CheckWinCondition;
            Stage = 1;
        }

        public void StartStage(int stageNum)
        {
            if (CanStartStage == false)
                return;

            if (DataManager.Instance.MonsterSpawnStageMap.ContainsKey(stageNum) == false)
                return;

            var stageList = DataManager.Instance.MonsterSpawnStageMap[stageNum];
            foreach (var stage in stageList)
            {
                CharBase charMonster = CharManager.Instance.CharGenerate(stage.MonsterID);
                TileManager.Instance.SetChar(stage.PositionIndex, charMonster);
            }

            TileManager.Instance.SwitchTileCombatmode(false);
            Debug.Log($"<color=red>새 스테이지 시작. StageNum = {stageNum}</color>");
        }

        public void StartCombat()
        {
            SetIsFinish(false);
            TileManager.Instance.SwitchTileCombatmode(true);
            CharManager.Instance.WakeAllCharAI();
            OnStartStage?.Invoke();
        }

        /// <summary>
        /// 이걸 호출할 때마다 다음 스테이지로 갈고예요 라는 뜻
        /// </summary>
        public void MoveToNextStage()
        {
            if(TryGetNextStage(Stage))
            {
                ItemManager.Instance.CleanupItems();
                CharManager.Instance.ReturnToOriginPos();
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
            // 전투 끝나고 한번만 체크하도록
            if (!IsStageFinished) IsStageFinished = true;
            else return;

            Debug.Log($"{charType} 타입의 모든 캐릭터가 제거되었습니다.");

            if (charType == typeof(CharPlayer))
            {
                Debug.Log("<color=#00FF22>모든 플레이어가 죽었습니다. 졌당..</color>");
                // 게임 오버 처리
            }
            else if (charType == typeof(CharMonster))
            {
                Debug.Log("<color=green>모든 적이 사라졌습니다. 이겼당!!</color>");
                CharManager.Instance.CopyFieldPlayerID();
                MoveToNextStage();
            }
        }

        /// <summary>
        /// 다음 스테이지 정보 가져오기
        /// </summary>
        public bool TryGetNextStage(int _stage)
        {
            //SynergyManager.Instance.Reset();
            CharManager.Instance.ClearAllChar();

            int nextStage = _stage + 1;
            if (DataManager.Instance.MonsterSpawnStageMap.ContainsKey(nextStage) == false)
            {
                return false;
            }

            return true;

        }

        public void SetIsFinish(bool isFinish)
        {
            IsStageFinished = isFinish;
        }
    }
}