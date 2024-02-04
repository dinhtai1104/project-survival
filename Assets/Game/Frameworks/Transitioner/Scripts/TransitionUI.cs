using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Transitioner
{
    public abstract class TransitionUI : MonoBehaviour
    {
        public abstract void Init();
        public abstract UniTask Trigger(Color color, float fadeInDuration);
        public abstract UniTask Release(float fadeOutDuration);
        
    }
}