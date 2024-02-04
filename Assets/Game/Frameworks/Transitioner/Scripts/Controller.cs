using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Transitioner
{
    public class Controller : MonoBehaviour
    {
        public static Controller Instance;

        [SerializeField]
        private TransitionUI block;
        [SerializeField]
        private float fadeInDuration = 0.5f, fadeOutDuration = 0.5f;
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                block = GetComponentInChildren<TransitionUI>(true);
                block.Init();
            }
        }
        [Button]
        public void Trigger()
        {
            Trigger(new Color(0, 0.3071372f, 0.3396226f, 1));
        }
        [Button]
        public void ReleaseTest()
        {
            Release();
        }
        public async UniTask Trigger(Color color)
        {
            await block.Trigger(color,fadeInDuration);
        }
        public async UniTask Release()
        {
            await block.Release(fadeOutDuration);
        }


    }
}

