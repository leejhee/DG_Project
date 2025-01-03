using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Client;

namespace Client
{
    public abstract class UI_Base : MonoBehaviour
    {
        /// <summary>
        /// 관리할 산하 오브젝트들
        /// </summary>
        protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

        float setWidth = 3200; // 사용자 설정 너비 (타겟 해상도 해당 해상도에서 비율로 조정)
        float setHeight = 1440; // 사용자 설정 높이 (타겟 해상도 해당 해상도에서 비율로 조정)
        private CanvasScaler canvasScaler;
        /// <summary>
        /// UI 최초 초기화
        /// </summary>
        public virtual void Init()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            SetResolution();
        }
        private void Awake()
        {
            Init();
        }
        /// <summary>
        /// 산하의 T type object들 _objects dictionary에 저장
        /// </summary>
        /// <typeparam name="T">해당 타입</typeparam>
        /// <param name="type">해당 타입 정보 가진 enum(각 UI에서 정의)</param>
        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(type);
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            _objects.Add(typeof(T), objects);

            for (int i = 0; i < names.Length; i++)
            {
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Util.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Util.FindChild<T>(gameObject, names[i], true);

                if (objects[i] == null)
                    Debug.LogError($"Failed to bind : {names[i]} on {gameObject.name}");
            }
        }

        /// <summary>
        /// bind된 object에서 원하는 object 얻기
        /// </summary>
        protected T Get<T>(int Index) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = null;
            if (_objects.TryGetValue(typeof(T), out objects) == false)
                return null;

            return objects[Index] as T;
        }
        #region Get_Override
        protected GameObject GetGameObject(int Index) => Get<GameObject>(Index);
        protected TMP_Text GetText(int Index) => Get<TMP_Text>(Index);
        protected Image GetImage(int Index) => Get<Image>(Index);
        protected Button GetButton(int Index) => Get<Button>(Index);
        #endregion Get_Override

        /// <summary>
        /// 해당 game object에 이벤트 할당
        /// </summary>
        /// <param name="action">할당할 이벤트</param>
        /// <param name="type">이벤트 발생 조건</param>
        public static void BindEvent(GameObject go, Action<PointerEventData> action, SystemEnum.eUIEvent type = SystemEnum.eUIEvent.Click)
        {
            UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

            switch (type)
            {
                case SystemEnum.eUIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                case SystemEnum.eUIEvent.Drag:
                    evt.OnDragHandler -= action;
                    evt.OnDragHandler += action;
                    break;
                case SystemEnum.eUIEvent.DragEnd:
                    evt.OnDragEndHandler -= action;
                    evt.OnDragEndHandler += action;
                    break;
            }
        }

        /// <summary> 반복문으로 사용하기 위한 index 사용 event 할당 </summary>
        public static void BindEvent(GameObject go, Action<PointerEventData, object> action, object pivot, SystemEnum.eUIEvent type = SystemEnum.eUIEvent.Click)
        {
            UI_PivotEventHandler evt = Util.GetOrAddComponent<UI_PivotEventHandler>(go);
            evt.Pivot = pivot;

            switch (type)
            {
                case SystemEnum.eUIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
            }
        }

        /// <summary>
        /// UI 비율 조정
        /// </summary>
        public void SetResolution()
        {
            float deviceWidth = Screen.width; // 기기 너비 저장
            float deviceHeight = Screen.height; // 기기 높이 저장

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(setWidth, setHeight);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            if (setWidth / setHeight < deviceWidth / deviceHeight)
            {
                canvasScaler.matchWidthOrHeight = 1f;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0f;
            }
        }

    }
}