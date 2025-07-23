using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class GameSceneCharInfo : MonoBehaviour
    {
        // 캐릭터 정보 UI를 전체 관리
        // 클릭한 캐릭터를 어느 ui에 띄울지 결정
        [SerializeField] CharacterInfoUI teamA_InfoUI;
        [SerializeField] CharacterInfoUI teamB_InfoUI;

        private void OnEnable()
        {
            MessageManager.SubscribeMessage<CharInfo>(this, OnCharClicked);
        }

        public void OnCharClicked(CharInfo info)
        {
            if (info.CharData.charType == SystemEnum.eCharType.ALLY)
                teamA_InfoUI.DisplayCharInfo(info);
            else if (info.CharData.charType == SystemEnum.eCharType.ENEMY)
                teamB_InfoUI.DisplayCharInfo(info);
        }

        // TODO: 초기 설정으로 아무 캐릭터.. (제일 작은 인덱스 타일에 잇는 캐릭터?)같은 애들 기본적으로 띄워줘야되나?
    }
}