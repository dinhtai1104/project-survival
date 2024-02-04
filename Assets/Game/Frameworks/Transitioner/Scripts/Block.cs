using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Transitioner
{
    public class Block : TransitionUI
    {
        [SerializeField]
        RectTransform _transform;
        Image [] images;
        [SerializeField]
        private AnimationCurve fadeCurve,moveCurve;
        [SerializeField]
        private Color color;

        Vector2 defaultSize;
        public override void Init()
        {
            images = GetComponentsInChildren<Image>();
            foreach(var image in images)
            {
                image.color = color;
            }
        }
        public override async UniTask Trigger(Color color,float fadeInDuration)
        {
            this.color = color;
            _transform.pivot = new Vector2(0.5f, 1);
            _transform.anchorMin = new Vector2(0.5f, 1);
            _transform.anchorMax = new Vector2(0.5f,1);
            _transform.anchoredPosition = Vector2.zero;
            gameObject.SetActive(true);

            if (defaultSize.x == 0)
            {
                defaultSize = GetComponent<RectTransform>().sizeDelta;
            }

            foreach (var image in images)
            {
                image.rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
                image.color = color;
            }
            float time = 0;
            while (time < fadeInDuration)
            {
                float progress = time / fadeInDuration;

                SetAlpha(fadeCurve.Evaluate(progress));
                Move(moveCurve.Evaluate(progress)*defaultSize.y);
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }
        }

        void SetAlpha(float a)
        {
            Color c;
            foreach(var image in images)
            {
                c = image.color;
                c.a = a;
                image.color = c;
            }
        }
        Vector2 size;
        void Move(float a)
        {
            size.x = _transform.sizeDelta.x;
            size.y = a;
            _transform.sizeDelta = size;
        }
        
        public override async UniTask Release(float fadeOutDuration)
        {
            _transform.pivot = new Vector2(0.5f, 0);
            _transform.anchorMin = new Vector2(0.5f, 0);
            _transform.anchorMax = new Vector2(0.5f, 0);
            _transform.anchoredPosition = Vector2.zero;
            float time = 0;
            while (time < fadeOutDuration)
            {
                float progress = time / fadeOutDuration;

                SetAlpha(fadeCurve.Evaluate(1-progress));
                Move(moveCurve.Evaluate(1-progress) * defaultSize.y);
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }
            gameObject.SetActive(false);
        }
    }
}