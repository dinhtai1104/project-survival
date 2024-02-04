using System;
using UnityEngine;

namespace Game.Effect
{
    public abstract class EffectAbstract : MonoBehaviour
    {
        protected Transform t, parent, followedTarget;
        Vector3 defaultScale;
        protected void Init()
        {
            CancelInvoke();
            if (t == null)
            {
                t = transform;
                parent = t.parent;
                defaultScale = t.localScale;

            }
            followedTarget = null;
            ClearParent();
        }
        Vector3 offset;
        public virtual EffectAbstract SetParent(Transform parent)
        {
            Init();
            offset = transform.position - parent.position;
            followedTarget = parent;
            return this;
        }
        private void Update()
        {
            if (followedTarget != null)
            {
                t.localPosition = followedTarget.position + offset;
            }
            if (!IsUsing())
            {
                Deactive();
            }
        }
        public void ClearParent()
        {
            if (followedTarget != null)
            {
                followedTarget = null;
                t.localScale = defaultScale;
            }
        }
        protected virtual void OnDisable()
        {
            ClearParent();
            //Invoke(nameof(ClearParent), 0.1f);
        }
        public virtual void Active() {
            gameObject.SetActive(true);

        }
        public virtual void Active(Transform parent) { }
        public virtual EffectAbstract Active(Vector3 pos, string text) { return this; }
        public virtual EffectAbstract Active(SpriteRenderer sr) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, float size) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, Color color) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, Sprite gunLeft, Sprite gunRight) { return this; }
        public virtual EffectAbstract Active(MeshRenderer renderer,float scale) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, Vector2 size) { return this; }
        public virtual EffectAbstract Active(Vector3 pos) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, Vector3 direction) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, int amount) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, int amount, bool isCritical) { return this; }
        public virtual EffectAbstract SetColor(Color color) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, string text,int direction) { return this; }
        public virtual EffectAbstract SetRotation(Vector3 rot) {  transform.localEulerAngles = rot; return this; }
        public abstract bool IsUsing();
        public virtual void Deactive()
        {
            gameObject.SetActive(false);
        }
        public void SetActive()
        {
            gameObject.SetActive(true);
        }

        public abstract void Stop();
    }
}