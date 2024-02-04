using com.mec;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltip
{
    public abstract class UIBaseTooltip : MonoBehaviour, IPointerClickHandler
    {
        protected RectTransform rectTransform;
        private bool isShowed = false;
        private float durationAutoHide = 0.5f;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();  
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isShowed) return;
            isShowed = true;
            ShowTooltip();
            Timing.RunCoroutine(_CooldownTooltip());
        }

        private IEnumerator<float> _CooldownTooltip()
        {
            yield return Timing.WaitForSeconds(durationAutoHide);
            isShowed = false;
        }

        public abstract void ShowTooltip();
    }
}