using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class GameSceneETC : MonoBehaviour
    {
        [SerializeField] string selectedColorCode = "#ff7f00";
        [SerializeField] string teamAColorCode = "#ffffcc";
        [SerializeField] string teamBColorCode = "#a6daf4";

        [SerializeField] Button BTN_TeamA;      // A팀 선택 버튼
        [SerializeField] Button BTN_TeamB;      // B팀 선택 버튼
        [SerializeField] Button BTN_GameStart;  // 베팅 완료 버튼

        [SerializeField] TextMeshProUGUI TMP_GameStart;  // 베팅 완료 버튼 위 텍스트
        [SerializeField] TextMeshProUGUI TMP_TotalGold;  // 보유 금액
        [SerializeField] TextMeshProUGUI TMP_Stake;      // 베팅 금액
        [SerializeField] TextMeshProUGUI TMP_RewardWin;  // 승리 시 보상
        [SerializeField] TextMeshProUGUI TMP_RewardLose; // 패배 시 보상

        [SerializeField] Slider slider;
        [SerializeField] ToastMessageUI toastMessege;

        Color selectedColor;
        Color teamAColor;
        Color teamBColor;

        string temp_start = "Start";
        string temp_end = "Finish";
        string temp_next = "Next!";

        void Start()
        {
            SetButtonColor();

            BTN_TeamA.onClick.AddListener(() => SelectTeam(typeof(CharPlayer)));
            BTN_TeamB.onClick.AddListener(() => SelectTeam(typeof(CharMonster)));

            slider.onValueChanged.AddListener(UpdateBetAmount);
            BTN_GameStart.onClick.AddListener(GameStart);

            StageManager.Instance.OnStageChanged += () => StartCoroutine(ToastMessage(temp_next));

            StageManager.Instance.OnStartCombat += SetInteractableFalse;
            //StageManager.Instance.OnStartCombat += () => StartCoroutine(ToastMessage(temp_start));

            StageManager.Instance.OnEndCombat += SetInteractableTrue;
            StageManager.Instance.OnEndCombat += PlayFinishCombatFlow;

            StageManager.Instance.OnGoldChanged += UpdateGoldText;

            UpdateTeamButtonVisuals();
            UpdateStartGameButton();
            UpdateGoldText(StageManager.Instance.Gold);

            toastMessege.gameObject.SetActive(false);
        }

        #region 준비단계
        void SetButtonColor()
        {
            selectedColor = Util.GetHexColor(selectedColorCode);
            teamAColor = Util.GetHexColor(teamAColorCode);
            teamBColor = Util.GetHexColor(teamBColorCode);
        }

        void SelectTeam(Type team)
        {
            StageManager.Instance.BetOnTeam(team);

            UpdateTeamButtonVisuals();
            UpdateStartGameButton();
        }

        void UpdateBetAmount(float value)
        {             
            int betAmount = Mathf.RoundToInt(value * StageManager.Instance.Gold);
            TMP_Stake.text = $"G{betAmount}";
            TMP_RewardWin.text = $"win : {betAmount * 2}";
            TMP_RewardLose.text = $"lose : 0";

            StageManager.Instance.BetStake(betAmount);
            UpdateStartGameButton();
        }

        void UpdateTeamButtonVisuals()
        {
            Type selectedTeam = StageManager.Instance.MyTeam;
            BTN_TeamA.image.color = (selectedTeam == typeof(CharPlayer)) ? selectedColor : teamAColor;
            BTN_TeamB.image.color = (selectedTeam == typeof(CharMonster)) ? selectedColor : teamBColor;
        }

        void UpdateStartGameButton()
        {
            // 1. 초기 + 팀 선택만 되어잇거나 베팅금액만 정햇거나 -> 게임 시작 불가
            // 버튼 텍스트 : 베팅 중

            // 2. 팀 선택되어 있고 베팅 금액이 0 이상 -> 시작 가능
            // 버튼 텍스트 : 전투 시작

            // 인터랙션은 가능한데, 경우에 따라 토스트메세지 떠야함
            // 팀 선택 안한 경우: “팀을 선택해주세요” 토스트 메세지 발생
            // 베팅 금액이 0인 경우: “배팅금액이 모자랍니다” 토스트 메세지 발생
            // 둘다면?

            // 일단 텍스트 상관없이 토스트메세지만 뜨도록
            // 근데 여기 아니고 버튼 이벤트에...

            bool isBetValid = StageManager.Instance.Stake > 0;

            Type selectedTeam = StageManager.Instance.MyTeam;
            bool isTeamSelected = selectedTeam != null;

            BTN_GameStart.interactable = isTeamSelected && isBetValid;
            TMP_GameStart.text = (isTeamSelected && isBetValid)? "Start" : "X";
            StageManager.Instance.SetIsBetted(isTeamSelected && isBetValid);
        }
        #endregion

        #region 전투단계 UI 흐름

        // 전투 시작
        // 화면 채도 낮춤 →‘ 전투 시작’ 토스트 메세지 애니메이션 →  화면 원상 복귀 → 전투 시작

        public void PlayStartCombatFlow()
        {
            StartCoroutine(StartCombatFlow());
        }

        private IEnumerator StartCombatFlow()
        {
            yield return StartCoroutine(ToastMessage(temp_start));

            StageManager.Instance.StartCombat();
        }



        // 전투 종료
        // 화면 채도 낮춤 →‘ 전투 종료’ 토스트 메세지 애니메이션 →  화면 원상 복귀 → 전투 종료

        public void PlayFinishCombatFlow()
        {
            StartCoroutine(FinishCombatFlow());
        }

        private IEnumerator FinishCombatFlow()
        {
            yield return StartCoroutine(ToastMessage(temp_end));

            yield return StartCoroutine(AnimateCoinIncrease(StageManager.Instance.Stake * 2));
        }

        #endregion

        #region 정산단계 UI 흐름

        // 화면 채도 낮춤 → ‘다음 라운드’ 토스트 메시지 애니메이션 →  필드 위의 기물 및 관련 UI가 모두 사라짐 → 화면 상단의 라운드 색 이동 → 토스트 애니메이션 사라짐 → 화면 원상 복귀 및 다음 라운드 기물 배치

        public void PlayNextStageFlow()
        {
            StartCoroutine(NextStageFlow());
        }

        private IEnumerator NextStageFlow()
        {
            yield return toastMessege.ShowMessageCoroutine(temp_next, () =>
            {
                // 중간 시점에 기물 제거
                CharManager.Instance.ClearAllChar();

                // TODO: UI 제거
            });

            StageManager.Instance.MoveToNextStage();
        }
        #endregion

        // 너무 구려
        void SetInteractableFalse()
        {
            BTN_GameStart.interactable = false;
            slider.interactable = false;
            BTN_TeamA.interactable = false;
            BTN_TeamB.interactable = false;
        }
        void SetInteractableTrue()
        {
            BTN_GameStart.interactable = true;
            slider.interactable = true;
            BTN_TeamA.interactable = true;
            BTN_TeamB.interactable = true;
        }
        void UpdateGoldText(int newGold)
        {
            TMP_TotalGold.text = $"G{newGold}"; 
        }

        void GameStart()
        {
            PlayStartCombatFlow();
        }

        IEnumerator ToastMessage(string txt)
        {
            toastMessege.gameObject.SetActive(true);

            yield return toastMessege.ShowMessageCoroutine(txt);

            toastMessege.gameObject.SetActive(false);
        }

        IEnumerator AnimateCoinIncrease(int amount)
        {
            if (amount <= 0) yield break;

            int startValue = StageManager.Instance.Gold;
            int endValue = StageManager.Instance.Gold + amount;
            float duration = 0.5f; // 애니메이션 시간 (초)
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                int displayValue = Mathf.FloorToInt(Mathf.Lerp(startValue, endValue, t));
                TMP_TotalGold.text = displayValue.ToString();
                yield return null;
            }

            // 보정
            StageManager.Instance.Gold = endValue;
            TMP_TotalGold.text = StageManager.Instance.Gold.ToString();
        }
    }
}