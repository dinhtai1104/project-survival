using UnityEngine;

namespace Game.Pool
{
    public class PoolObject:MonoBehaviour
    {
        public delegate void OnReleased(PoolObject objectBase);
        public OnReleased onReleased;
        public delegate void OnReEnabled(PoolObject objectBase);
        public OnReEnabled onReEnabled;


        private bool isAvailable;
        public bool IsAvailable
        {
            get { return isAvailable; }
            set
            {
                isAvailable = value;

            }
        }

        protected virtual void OnDisable()
        {
            IsAvailable = true;
            onReleased?.Invoke(this);
        }
        protected virtual void OnEnable()
        {
            IsAvailable = false;
            onReEnabled?.Invoke(this);
        }
    }

}