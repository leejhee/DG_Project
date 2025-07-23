using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Client
{
    public class CharacterInfoUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI TMP_Name;

        /// <summary> 캐릭터 기본 정보 </summary>
        public void DisplayCharInfo(CharInfo charInfo)
        {
            // 호출은 캐릭터 클릭 시에만 하면 됨
            TMP_Name.text = charInfo.charBase.CharData.charName;
        }

        /// <summary> 전투 중에 계속 변경되는 스탯들 업데이트 </summary>
        public void UpdateCharInfo(CharStat charStat)
        {
            // TODO : 이름 띄우기 구현 후에 추가
            // 파라미터 바꿀까 CharInfo로...?
        }
    }
}