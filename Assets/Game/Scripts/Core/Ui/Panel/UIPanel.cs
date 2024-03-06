using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Ui.View
{
    public abstract class Panel : UnityEngine.MonoBehaviour
    {
        //public List<Transform> listScaleTrans;
        protected Animator ani;
        protected Transform _transform;
        public bool overrideBack = false, isPersistant = false;
        public System.Action onClosed;

        private CancellationTokenSource m_Cts;
        [ShowInInspector]
        private ITransitionElement[] transitions;

        public virtual void Clear()
        {
            //OnDestroy();
        }
        public virtual void OnDestroy()
        {

        }
        public virtual void Destroy()
        {
            Destroy(gameObject);
            //Addressables.ReleaseInstance(gameObject);
        }
        [Button]
        public virtual void Show(params object[] @params)
        {
            Prepare();
            Active();
        }
        

        public void ClearCloseTrigger()
        {
            ani.ResetTrigger("Close");
        }
        public virtual async UniTask Hide()
        {
            if (transitions != null && transitions.Length > 0)
            {
                await HideByTransitions();
                m_Cts.Cancel();
                Deactive();
                return;
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
                if (panelFadeAnimation != null)
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
                    list.Add(transition.AutoHide().AttachExternalCancellation(m_Cts.Token));
                }
                await UniTask.WhenAll(list).AttachExternalCancellation(m_Cts.Token);
            }
        }

        public virtual void Deactive()
        {
            gameObject.SetActive(false);
            PanelManager.Instance.OnPanelHidden(this);
            PanelManager.Instance.Unregister(this);
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
            PanelManager.Instance.OnPanelShown(this);
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
                list.Add(transition.AutoShow().AttachExternalCancellation(m_Cts.Token));
            }
            await UniTask.WhenAll(list).AttachExternalCancellation(m_Cts.Token);
        }

        public virtual void ShowAfterAd() { }
        public virtual UniTask Close()
        {
            this.onClosed?.Invoke();
            return Hide();
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

        public virtual void PostInit()
        {
            m_Cts = new CancellationTokenSource();
        }

        public virtual void OnBack()
        {
            if (overrideBack) return;
            Close();
        }
    }
}
