using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Transitioner
{
    public class FadeBlock : TransitionUI
    {
        [SerializeField]
        RectTransform _transform;
        [SerializeField]
        private Image backGroundImg;
        [SerializeField]
        private AnimationCurve curve;
        [SerializeField]
        private Color color;

        public override void Init()
        {
        
        }
        public override async UniTask Trigger(Color color, float fadeInDuration)
        {
            gameObject.SetActive(true);
            this.color = color;
            backGroundImg.color = color;
            float time = 0;
            while (time < fadeInDuration)
            {
                float progress = time / fadeInDuration;

                SetSize(curve.Evaluate(progress));
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }
            SetSize(curve.Evaluate(1));
        }

        Vector3 size = Vector3.one;
        void SetSize(float a)
        {
            size.x = size.y = a;
            _transform.localScale = size;
        }
     

        public override async UniTask Release(float fadeOutDuration)
        {
           
            float time = 0;
            while (time < fadeOutDuration)
            {
                float progress = time / fadeOutDuration;

                SetSize(curve.Evaluate(1 - progress));
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }
            gameObject.SetActive(false);
        }
    }
}