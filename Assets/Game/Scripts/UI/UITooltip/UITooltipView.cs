using com.mec;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltip
{
    public class UITooltipView : MonoBehaviour
    {
        private RectTransform rectTransform;

        [SerializeField] private Image iconTooltip;
        [SerializeField] private TextMeshProUGUI headerTxt;
        [SerializeField] private TextMeshProUGUI descriptionTxt;
        [SerializeField] private LayoutElement elementLayoutDescription;

        public void SetIcon(Sprite sprite) { iconTooltip.sprite = sprite; }
        public void SetHeader(string header) { headerTxt.text = header; }
        public void SetDescription(string description)
        {
            descriptionTxt.text = description;
            if (!string.IsNullOrEmpty(description))
            {
                elementLayoutDescription.enabled = (description.Length > 100);
            }
        }

        public void SetData(string header = "", string description = "", Sprite icon = null)
        {
            SetIcon(icon);
            SetHeader(header);
            SetDescription(description);
            Show();
        }
        public void SetData(TooltipData data)
        {
            Timing.KillCoroutines(gameObject);
            SetIcon(data.icon);
            SetHeader(data.header);
            SetDescription(data.description);
            Show();
        }

        private void Show()
        {
            Timing.KillCoroutines(gameObject);
            iconTooltip.gameObject.SetActive(iconTooltip.sprite != null);
            headerTxt.gameObject.SetActive(!string.IsNullOrEmpty(headerTxt.text));
            descriptionTxt.gameObject.SetActive(!string.IsNullOrEmpty(descriptionTxt.text));
            rectTransform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutBack).SetId(gameObject).SetUpdate(true);

            Timing.RunCoroutine(WaitForPool(), Segment.RealtimeUpdate, gameObject);
        }

        private IEnumerator<float> WaitForPool()
        {
            yield return Timing.WaitForSeconds(2f);
            rectTransform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetId(gameObject).SetUpdate(true);
            yield return Timing.WaitForSeconds(0.25f);
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            Timing.KillCoroutines(gameObject);
            DOTween.Kill(gameObject);
        }

        public void SetPivot(Vector2 pivot)
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            rectTransform.pivot = pivot;
        }
    }
}