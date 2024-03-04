using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneManger.Loading
{
    public abstract class BaseLoadingScene : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public virtual void StartLoading(BaseSceneManager sceneManager)
        {
        }

        public virtual void EndLoading()
        {
        }

        public abstract void LoadingProgress(float percentage);
    }
}
