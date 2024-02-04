using UnityEngine;

namespace Game.Pool
{
    public class UIPoolComponent : MonoBehaviour
    {
        UIPoolHandler poolHandler;
        public void OnInitialized(UIPoolHandler poolHandler)
        {
            this.poolHandler = poolHandler;
        }
        protected virtual void OnDestroy()
        {
            poolHandler.Release(this);
        }
        protected virtual void OnDisable()
        {
            poolHandler.Release(this);
        }

    }
}