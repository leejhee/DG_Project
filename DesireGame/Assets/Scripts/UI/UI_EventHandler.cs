using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client
{
    /// <summary> 매게변수 없는 이벤트 핸들러 </summary>
    public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler
    {
        // 클릭 
        public Action<PointerEventData> OnClickHandler = null;
        // 드래그 중
        public Action<PointerEventData> OnDragHandler = null;
        // 드래그 끝
        public Action<PointerEventData> OnDragEndHandler = null;
        //.... 필요시 추가 구현 + UI_Base BindEvent구성 바람

        // 클릭 
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickHandler?.Invoke(eventData);
        }

        // 드래그 중
        public void OnDrag(PointerEventData eventData)
        {
            OnDragHandler?.Invoke(eventData);
        }

        // 드래그 끝
        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEndHandler?.Invoke(eventData);
        }
    }
}