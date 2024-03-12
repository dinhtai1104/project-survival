using com.mec;
using DG.Tweening;
using Mosframe;
using Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManger.Transition
{
    public class FadeTransition : BaseSceneTransition
    {
        [SerializeField, Range(0f, 10f)] private float _duration;

        [SerializeField] private FadeMode _fadeMode;

        private Image _image;

        private enum FadeMode
        {
            FadeIn = 0,
            FadeOut = 1
        }

        private void Awake()
        {
            _image = GetComponentInChildren<Image>();
        }

        public override void StartTransition()
        {
            base.StartTransition();
            float startAlpha = 0f;
            float endAlpha = 0f;

            switch (_fadeMode)
            {
                case FadeMode.FadeIn:
                    startAlpha = 0f;
                    endAlpha = 1f;
                    break;
                case FadeMode.FadeOut:
                    startAlpha = 1f;
                    endAlpha = 0f;
                    break;
            }

            // Set start alpha
            var color = _image.color;
            color.a = startAlpha;
            _image.color = color;

            var diff = Mathf.Abs(endAlpha - startAlpha);

            Timing.RunCoroutine(_Fading(startAlpha, endAlpha, diff));

        }

        private IEnumerator<float> _Fading(float startAlpha, float endAlpha, float diff)
        {
            float alpha = startAlpha;
            float time = 0;
            var color = _image.color;

            while (time < _duration)
            {
                alpha = Mathf.Lerp(startAlpha, endAlpha, time);
                time += Time.deltaTime;

                Progress = Mathf.Abs(alpha - startAlpha) / diff;
                color.a = alpha;
                _image.color = color;

                yield return Timing.DeltaTime;
            }

            IsDone = true;
            Progress = 1f;
        }

        protected override void EndTransition()
        {
            base.EndTransition();
            PoolFactory.Despawn(gameObject);
        }
    }
}