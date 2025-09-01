using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using TMPro;

namespace Client
{
    public class StageManager : Singleton<StageManager>
    {
        private StageManager() { }


        public int Stage { get; private set; }         // 현재 스테이지
        public Type MyTeam { get; private set; }       // 베팅한 팀
        public int Stake { get; private set; }         // 베팅 금액

        // 스테이지 새로 시작 가능 상태 ->  true이면 스테이지 새로 배치
        public bool CanStartStage { get; private set; } = true;

        // 베팅 완료했는지 -> true이면 게임시작 가능
        public bool IsBetted { get; private set; } = false;

        // 전투가 끝났는지
        public bool IsStageFinished { get; private set; } = false;
        
        // 전투 직후 다음 스테이지로 진행하는지
        public bool IsStageStartedImmediately { get; private set; }
        
        public Action OnStageChanged;
        public Action OnStartCombat;
        public Action OnEndCombat;
        public Action<int> OnGoldChanged;

        private int gold = 500; // 보유 골드 - 초기값 아무거나
        public int Gold
        {
            get => gold;
            set
            {
                if (gold != value)
                {
                    gold = value;
                    OnGoldChanged?.Invoke(gold);
                }
            }
        }

        public override void Init()
        {
            base.Init();
            CharManager.Instance.OnCharTypeEmpty += CheckWinCondition;
            SynergyManager.Instance.Init(); // 이제 시너지는 스테이지 배치 로직에 의존하는 시스템이다.
        }

        /// <summary> 새로운 스테이지 배치 </summary>
        public void StartStage(int stageNum)
        {
            CharManager.Instance.HardClearAll();

            if (!CanStartStage)
                return;

            if (DataManager.Instance.CharacterSpawnStageMap.ContainsKey(stageNum) == false)
                return;

            SetIsBetted(false);
            OnStageChanged?.Invoke();

            var stageList = DataManager.Instance.CharacterSpawnStageMap[stageNum];
            foreach (var stage in stageList)
            {
                CharBase charMonster = CharManager.Instance.CharGenerate(stage.CharacterID);
                TileManager.Instance.SetChar(stage.PositionIndex, charMonster);
            }
            //캐릭터 배치 후 시너지 분배
            SynergyManager.Instance.RebuildFromFieldAndDistribute();
            
            Stage = stageNum;
            TileManager.Instance.SwitchTileCombatmode(false);
            OnEndCombat?.Invoke();
            Debug.Log($"<color=red>새 스테이지 시작. StageNum = {stageNum}</color>");
        }

        public void StartCombat()
        {
            if (!IsBetted)
            {
                Debug.LogError("<color=red>! 베팅 완료 후 전투 시작이 가능합니다.</color>");
                return;
            }
                
            SetIsFinish(false);
            TileManager.Instance.SwitchTileCombatmode(true);
            CharManager.Instance.WakeAllCharAI();
            OnStartCombat?.Invoke(); 
        }

        /// <summary>
        /// 이걸 호출할 때마다 다음 스테이지로 갈고예요 라는 뜻
        /// </summary>
        public void MoveToNextStage()
        {
            if (Stage % 5 == 0)
            {
                // 5의 배수 라운드때마다 체크
                // 정산금보다 보유금이 더 많은지 확인
                // true 넘어감 false 게임 종료
                StageData currentStageData = DataManager.Instance.GetData<StageData>(Stage);
                if(currentStageData== null) return;

                bool canPassStage = Gold >= currentStageData.requiredCredit;

                if (!canPassStage) return; // 패배 띄우기
            }

            if (TryGetNextStage(Stage))
            {
                //ItemManager.Instance.CleanupItems();
                //CharManager.Instance.ReturnToOriginPos();
                StartStage(++Stage);
            }
            else
            {
                Debug.LogError("다음 스테이지 없음");
                return;
            }
        }

        /// <summary> 승패 판정 </summary>
        public void CheckWinCondition(Type charType)
        {
            // 전투 끝나고 한번만 체크하도록
            if (!IsStageFinished) SetIsFinish(true);
            else return;

            Debug.Log($"{charType} 타입의 모든 캐릭터가 제거되었습니다.");
            CharManager.Instance.SleepAllCharAI(); // AI 전부 turnoff
            // 제거된 타입이 내가 베팅한 팀이 아닌 경우 -> 승리
            bool isWin = MyTeam != charType;

            GetReward(isWin);
            
            if(IsStageStartedImmediately)
                MoveToNextStage();
        }

        /// <summary>
        /// 다음 스테이지 정보 가져오기
        /// </summary>
        public bool TryGetNextStage(int _stage)
        {
            //SynergyManager.Instance.Reset();
            CharManager.Instance.ClearAllChar();

            int nextStage = _stage + 1;
            if (DataManager.Instance.CharacterSpawnStageMap.ContainsKey(nextStage) == false)
            {
                return false;
            }

            return true;

        }
        /// <summary> 어떤 팀의 승리에 베팅? </summary>
        public void BetOnTeam(Type type)
        {
            MyTeam = type;
        }

        /// <summary> 베팅 금액 정함 </summary>
        public void BetStake(int gold)
        {
            if (gold > Gold)
            {
                Debug.LogError("보유 금액 초과");
                return;
            }

            Stake = gold;
        }

        private void GetReward(bool iswin)
        {
            if(iswin)
            {
                Debug.Log("<color=green>내가 베팅한 팀이 승리. 이겼당!!</color>");
                Gold += Stake * 2;
            }
            else
            {
                Debug.Log("<color=#00FF22>상대팀이 승리. 졌당..</color>");
                // 패배 시 0원
            }
        }
        
        public void SetIsFinish(bool isFinish)
        {
            IsStageFinished = isFinish;
        }
        public void SetIsBetted(bool isBetted)
        {
            IsBetted = isBetted;
        }
    }
}