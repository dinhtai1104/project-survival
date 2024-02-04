using Cysharp.Threading.Tasks;
using Game.Effect;
using TMPro;
using UnityEngine;

namespace Effect
{
    public class EffectText : EffectAbstract
    {
        [SerializeField]
        private TMPro.TextMeshPro effectText;
        [SerializeField]
        private float  shakeTime = 0.1f,flyRate=1;
        [SerializeField]
        private int killTime = 200;
        public override EffectAbstract Active(Vector3 pos, string text)
        {
            SetColor(Color.white);
            effectText.text = text;
            transform.position = pos;
            transform.localScale = Vector2.one;
            gameObject.SetActive(true);
            PlayEffect().Forget();
            return this;
        }
        public override EffectAbstract SetColor(Color color)
        {
            effectText.color = color;
            return this;
        }
        async UniTaskVoid PlayEffect()
        {
            Fly(transform);

            UniTask shakeTask = Shake(transform, shakeTime);
            await shakeTask;
            await UniTask.Delay(killTime );
            await Hide(effectText);
            gameObject.SetActive(false);
        }
        async UniTask Shake(Transform transform, float time, float power = 1)
        {
            float t = time;
            Vector3 scale = transform.localScale;
            float a = 0;
            while (a < Mathf.PI * 4 / 4f)
            {
                scale.x = 1f + Mathf.Sin(Mathf.PI / 2 - a) * 0.3f;
                scale.y = 1 - Mathf.Sin(Mathf.PI / 2 - a) * 0.3f;
                transform.localScale = scale;
                a += Mathf.PI / 6;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
            while (scale.x < 0.99f || scale.y < 0.99f)
            {
                scale.x = Mathf.Lerp(scale.x, 1, 0.4f);
                scale.y = Mathf.Lerp(scale.y, 1, 0.4f);
                transform.localScale = scale;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }
        async UniTask Hide(TextMeshPro text)
        {
            float a = Mathf.PI / 2f;
            Color c = text.color;

            Vector3 scale = transform.localScale;
            while (c.a > 0)
            {
                c.a -= Time.fixedDeltaTime * 6;
                text.color = c;
                scale.x = Mathf.Lerp(scale.x, 0, 0.05f);
                scale.y = Mathf.Lerp(scale.y, 0, 0.05f);
                transform.localScale = scale;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }
        async UniTask Fade(TextMeshPro text)
        {
            Color c = text.color;
            c.a = 0;
            while (c.a < 1)
            {
                c.a += Time.fixedDeltaTime * 15;
                text.color = c;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }
        async UniTask Fly(Transform transform)
        {
            Vector3 move = new Vector3(0,flyRate);
            while (gameObject.activeSelf)
            {
                transform.localPosition += move * Time.fixedDeltaTime;
                move.y -= Time.fixedDeltaTime ;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }


        public override bool IsUsing()
        {
            return gameObject.activeSelf;
        }
        public override void Stop()
        {
        }
    }
}