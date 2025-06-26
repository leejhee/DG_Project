using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;

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

        Color selectedColor;
        Color teamAColor;
        Color teamBColor;

        void Start()
        {
            SetButtonColor();
            BTN_TeamA.onClick.AddListener(() => SelectTeam(typeof(CharPlayer)));
            BTN_TeamB.onClick.AddListener(() => SelectTeam(typeof(CharMonster)));

            slider.onValueChanged.AddListener(UpdateBetAmount);
            BTN_GameStart.onClick.AddListener(GameStart);

            StageManager.Instance.OnStartCombat += SetInteractableFalse;
            StageManager.Instance.OnEndCombat += SetInteractableTrue;
            StageManager.Instance.OnGoldChanged += UpdateGoldText;

            UpdateTeamButtonVisuals();
            UpdateStartGameButton();
            UpdateGoldText(StageManager.Instance.Gold);
        }

        /// <summary> 미리 헥사코드에 맞는 색상 지정해서 설정해두기 </summary>
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
            StageManager.Instance.StartCombat();
        }
    }
}