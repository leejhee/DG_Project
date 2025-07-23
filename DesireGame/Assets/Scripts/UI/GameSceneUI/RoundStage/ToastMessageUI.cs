using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

namespace Client
{
    public class ToastMessageUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txt;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] CanvasGroup panel;
        [SerializeField] float fadeInOutTime = 0.5f;

        //private void Start()
        //{
        //    canvasGroup.alpha = 0f;
        //}
        private IEnumerator fadeInOut(CanvasGroup target, float durationTime, bool inOut)
        {
            float start, end;
            if (inOut)
            {
                start = 0f;
                end = 1f;
            }
            else
            {
                start = 1f;
                end = 0f;
            }

            Color current = Color.clear; /* (0, 0, 0, 0) = 검은색 글자, 투명도 100% */
            float elapsedTime = 0.0f;

            while (elapsedTime < durationTime)
            {
                float alpha = Mathf.Lerp(start, end, elapsedTime / durationTime);

                target.alpha = alpha;

                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }


        public IEnumerator ShowMessageCoroutine(string msg, Action onMidpoint = null)
        {
            // 1. 화면 채도 낮춤
            yield return fadeInOut(panel, fadeInOutTime, true);

            Color originalColor = txt.color;
            txt.text = msg;
            txt.enabled = true;

            // 2. 토스트 메시지 애니메이션
            yield return fadeInOut(canvasGroup, fadeInOutTime, true);

            onMidpoint?.Invoke();


            float elapsedTime = 0.0f;
            while (elapsedTime < fadeInOutTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 3. 화면 원상 복귀
            yield return fadeInOut(canvasGroup, fadeInOutTime, false);

            txt.enabled = false;
            txt.color = originalColor;
        }
    }
}