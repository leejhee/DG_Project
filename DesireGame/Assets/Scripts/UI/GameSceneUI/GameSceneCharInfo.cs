using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class GameSceneCharInfo : MonoBehaviour
    {
        // 캐릭터 정보 UI를 전체 관리
        [SerializeField] CharacterInfoUI teamA_InfoUI;
        [SerializeField] CharacterInfoUI teamB_InfoUI;

        private void OnEnable()
        {
            MessageManager.SubscribeMessage<CharInfo>(this, OnCharClicked);

            teamA_InfoUI.gameObject.SetActive(false);
            teamB_InfoUI.gameObject.SetActive(false);
        }
        private void OnDisable()
        {
            MessageManager.RemoveMessageAll(this);
        }

        public void OnCharClicked(CharInfo info)
        {
            // 클릭한 캐릭터 타입에 따라 어느 ui에 띄울지 결정
            // 타입 구분이 잘 안 되는 것 같은데?
            //SystemEnum.eCharType etype = info.charBase.GetCharType();
            //Debug.Log($"<color=yellow>클릭한 캐릭터의 타입 : {etype}</color>");

            System.Type type = info.charBase.GetType();

            //if (etype == SystemEnum.eCharType.ALLY)
            //{
            //    teamA_InfoUI.gameObject.SetActive(true);
            //    teamA_InfoUI.DisplayCharInfo(info);

            //}
            //else if (etype == SystemEnum.eCharType.ENEMY)
            //{
            //    teamB_InfoUI.gameObject.SetActive(true);
            //    teamB_InfoUI.DisplayCharInfo(info);

            //}
            if (type == typeof(CharPlayer))
            {
                teamA_InfoUI.gameObject.SetActive(true);
                teamA_InfoUI.DisplayCharInfo(info);

            }
            else if (type == typeof(CharMonster))
            {
                teamB_InfoUI.gameObject.SetActive(true);
                teamB_InfoUI.DisplayCharInfo(info);

            }

        }
    }
}