using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Level
{
    public abstract class LevelBuilderBase : MonoBehaviour
    {
        protected CancellationTokenSource cancellation;

        protected virtual void OnEnable()
        {
            cancellation = new CancellationTokenSource();
        }
        protected virtual void OnDisable()
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
            }

        }
        protected virtual void OnDestroy()
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
            }
            Destroy();
        }

        public abstract void Destroy();
        public abstract UniTask<Map> SetUp(string mapId);
        public abstract UniTask<BackGround> SetUpBackGround( string backGroundId);
        public virtual async UniTask<Portal> SetUpPortal(bool isBuffPortal) { return null; }

        public virtual Map CurrentMap() { return null; }
        public virtual GroupNpcSpawn GetNpcSpawns() { return null; }
    }
}