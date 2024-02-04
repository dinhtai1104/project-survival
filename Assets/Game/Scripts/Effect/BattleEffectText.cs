using Cysharp.Threading.Tasks;
using Game.Effect;
using GameUtility;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Effect
{
    public class BattleEffectText : EffectAbstract
    {
        [SerializeField]
        private TMPro.TextMeshPro effectText;
        [SerializeField]
        private float dropRate = 1,shakeTime=0.1f,bounceOffset=0.1f,slowRate=1;
        [SerializeField]
        private Vector2 moveOffset;
        [SerializeField]
        private int killTime = 200;
        [SerializeField]
        private Color baseColor;
        public EffectAbstract Active(Vector3 pos, string text,int direction,Color color)
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                cancellation = null;
            }
            cancellation = new CancellationTokenSource();
            effectText.color = color;
            effectText.text = text;
            transform.position = pos;
            transform.localScale = Vector2.one;
            gameObject.SetActive(true);
            PlayEffect(direction).Forget();
            return this;
        }
        CancellationTokenSource cancellation ;

        private void OnEnable()
        {
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if(cancellation!=null)
            cancellation.Cancel();
        }

        private void OnDestroy()
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
            }
        }
        async UniTaskVoid PlayEffect(int direction)
        {
            Stray(transform,direction);

            await Shake(transform, shakeTime);
            await Hide(effectText);
            gameObject.SetActive(false);
        }
        async UniTask Shake(Transform transform, float time, float power = 1)
        {
            float t = time;
            Vector3 scale = transform.localScale;
            float a = 0;
            while (a<Mathf.PI*4/4f)
            {
                scale.x = 1f + Mathf.Sin(Mathf.PI/2-a) *bounceOffset;
                 scale.y = 1 - Mathf.Sin(Mathf.PI/2-a) *bounceOffset;
                transform.localScale = scale;
                a += Mathf.PI / 6;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate,cancellationToken:cancellation.Token);
            }
            while (scale.x<0.99f || scale.y < 0.99f)
            {
                scale.x =  Mathf.Lerp(scale.x, 1, 0.4f);
                scale.y = Mathf.Lerp(scale.y, 1, 0.4f);
                transform.localScale = scale;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellation.Token);
            }
        }
        async UniTask Hide(TextMeshPro text)
        {
            float a = Mathf.PI / 2f;
            Color c = text.color;

            Vector3 scale = transform.localScale;
            while (c.a>0)
            {
                c.a -= Time.fixedDeltaTime*6;
                text.color = c;
                scale.x = Mathf.Lerp(scale.x, 0, 0.05f);
                scale.y = Mathf.Lerp(scale.y, 0, 0.05f);
                transform.localScale = scale;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellation.Token);
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
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellation.Token);
            }
        }
        async UniTask Stray(Transform transform,int direction)
        {
            Vector3 move = moveOffset * UnityEngine.Random.Range(0.95f, 1.05f);
            move.x *= direction;
            while (gameObject.activeSelf)
            {
                transform.localPosition += move*Time.fixedDeltaTime;
                move = Vector3.Lerp(move, Vector2.zero, slowRate);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellation.Token);
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