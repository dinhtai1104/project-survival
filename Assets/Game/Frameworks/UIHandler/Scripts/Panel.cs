using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI
{
    public abstract class Panel : UnityEngine.MonoBehaviour
    {
        //public List<Transform> listScaleTrans;
        protected Animator ani;
        protected Transform _transform;
        public bool overrideBack = false,isPersistant=false;
        public System.Action onClosed;

        [ShowInInspector]
        private ITransitionElement[] transitions;

        public virtual void Clear()
        {
            //OnDestroy();
        }
        public virtual void OnDestroy()
        {
            
        }
        public void Destroy()
        {
            Addressables.ReleaseInstance(gameObject);
        }
        [Button]
        public virtual void Show()
        {
            Prepare();
            Active();
        }
        public void ClearCloseTrigger()
        {
            ani.ResetTrigger("Close");
        }
        public virtual async void Hide()
        {
            if (transitions != null && transitions.Length > 0)
            {
                await HideByTransitions();
            }

            if (ani == null)
            {
                ani = GetComponent<Animator>();
            }

            if (ani != null)
            {
                ani.SetTrigger("Close");
            }
            else
            {
                PanelFadeAnimation panelFadeAnimation = GetComponent<PanelFadeAnimation>();
                if(panelFadeAnimation!=null)
                    panelFadeAnimation.Close(() => Deactive());
                else
                    Deactive();
            }
        }
        List<UniTask> list = new List<UniTask>();

        [Button]
        public async UniTask HideByTransitions()
        {
            if (transitions != null)
            {
                list.Clear();
                foreach (var transition in transitions)
                {
                    list.Add(transition.AutoHide());
                }
                await UniTask.WhenAll(list);
            }
        }

        public virtual void Deactive()
        {
            gameObject.SetActive(false);
            UI.PanelManager.Instance.OnPanelHidden(this);
            PanelManager.Instance.DeRegister(this);


            Destroy();

        }
        public virtual void Active()
        {
            PanelFadeAnimation panelFadeAnimation = GetComponent<PanelFadeAnimation>();
            if (panelFadeAnimation != null)
            {
                panelFadeAnimation.Show();
            }
            else
            {
                gameObject.SetActive(true);
            }
            UI.PanelManager.Instance.OnPanelShown(this);
            PanelManager.Instance.Register(this);

            if (transitions != null && transitions.Length > 0)
            {
                ShowByTransitions();
            }
        }

        [Button]
        public virtual async void ShowByTransitions()
        {
            List<UniTask> list = new List<UniTask>();
            foreach (var transition in transitions)
            {
                list.Add(transition.AutoShow());
            }
            await UniTask.WhenAll(list);
        }

        public virtual void ShowAfterAd() { }
        public virtual void Close()
        {
            this.onClosed?.Invoke();
            Hide();
        }
        public virtual void Prepare()
        {
            GetComponentInChildren<SafeAreaHandler>()?.Safe();
            transitions = GetComponentsInChildren<ITransitionElement>(true);
            foreach (var trans in transitions)
            {
                trans.Init();
            }
        }
        public abstract void PostInit();

        public virtual void OnBack()
        {
            if (overrideBack) return;
            Close();
        }
        public void PlaySFX(AnimationEvent animationEvent)
        {
            Sound.Controller.Instance.PlayOneShot((AudioClip)animationEvent.objectReferenceParameter);
        }
    }
}