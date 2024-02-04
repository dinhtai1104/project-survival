using UnityEngine;

namespace Game.GameActor
{
    public abstract class MoveAddOn:ScriptableObject
    {
        public virtual MoveAddOn SetUp(MoveHandler handler)
        {
            MoveAddOn instance = (MoveAddOn)CreateInstance(GetType());
            instance.Init(handler);
            return instance;
        }
        public abstract void Init(MoveHandler handler);
        public abstract void OnUpdate(MoveHandler handler);
        public abstract void OnFixedUpdate(MoveHandler handler);

    }
}