using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Client
{
    public class StageUI : MonoBehaviour
    {
        [SerializeField] List<RoundIcon> roundIconList;
        [SerializeField] TextMeshProUGUI requiredGold;

        int currentIndex;
        private void Start()
        {
            StageManager.Instance.OnStageChanged += OnStageChanged;
            RenewStageUI();
        }

        public void ToggleRoundIcon()
        {
            // 라운드 아이콘 하나씩 표시할 때 사용하는 함수

            roundIconList[currentIndex]?.RevertIcon();

            int realStage = StageManager.Instance.Stage % 1600000;
            if (realStage == 0) realStage = 1600000;

            currentIndex = (realStage - 1) % 5;
            roundIconList[currentIndex].HighlightIcon();
        }
        public void RenewStageUI()
        {
            // 실제 스테이지 번호 계산
            int realStage = StageManager.Instance.Stage % 1600000;  // 예: 1600001 -> 1

            // 현재 라운드 세트의 시작 번호 계산 (5개씩 묶음)
            int startStageNumber = ((realStage - 1) / 5) * 5 + 1;

            for (int i = 0; i < roundIconList.Count; i++)
            {
                int displayStage = startStageNumber + i;
                roundIconList[i].SetStageNumber(displayStage);
            }

            // 현재 인덱스 재설정
            ToggleRoundIcon();
        }

        public void OnStageChanged()
        {
            int realStage = StageManager.Instance.Stage % 1000000;

            // 새로운 라운드 세트로 넘어가는 경우
            if ((realStage - 1) % 5 == 0)
            {
                RenewStageUI();      // 새 묶음: 6, 11, 16 등
            }
            else
            {
                ToggleRoundIcon();   // 기존 묶음 내에서 이동: 1 → 2, 6 → 7 등
            }
        }
    }
}