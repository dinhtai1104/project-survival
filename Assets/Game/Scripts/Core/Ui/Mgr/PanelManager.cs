using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Ui.View
{
    public class PanelManager : UnityEngine.MonoBehaviour
    {
        public delegate void OnPanelShow(Panel panel);
        public static OnPanelShow onPanelShow;
        public static PanelManager Instance;
        [SerializeField]
        private List<Panel> panels = new List<Panel>();
        private Stack<Panel> stack = new Stack<Panel>();

        // Start is called before the first frame update
        void Awake()
        {
            Init();
        }
        private void OnDestroy()
        {
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].Clear();
                panels[i] = null;
            }
        }
        public static void Create(Type type, System.Action<Panel> onLoaded)
        {
            Create(type.ToString(), onLoaded);
        }
        public static void Create(string type, System.Action<Panel> onLoaded)
        {
            CreateAsync(type).ContinueWith(onLoaded).Forget();
        }
        public static async UniTask<Panel> CreateAsync(string type)
        {
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(type, PanelManager.Instance.transform);
            await op;
            Panel panel = op.Result.GetComponent<Panel>();
            panel.Prepare();
            panel.PostInit();
            return panel;
        }
        public static async UniTask<Panel> CreateAsync(Type type)
        {
            return await CreateAsync(type.ToString());
        }
        public static async UniTask<TPanel> CreateAsync<TPanel>() where TPanel : Panel
        {
            var type = typeof(TPanel);
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(type.ToString(), PanelManager.Instance.transform);
            await op;
            TPanel panel = op.Result.GetComponent<TPanel>();
            panel.Prepare();
            panel.PostInit();
            return panel;
        }
        public static async UniTask<TPanel> CreateAsync<TPanel>(string address) where TPanel : Panel
        {
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(address, PanelManager.Instance.transform);
            await op;
            TPanel panel = op.Result.GetComponent<TPanel>();
            panel.Prepare();
            panel.PostInit();
            return panel;
        }
        public static async UniTask<TPanel> CreateAsync<TPanel>(string address, System.Action<float> onLoading, System.Action onLoaded) where TPanel : Panel
        {
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(address, PanelManager.Instance.transform);
            while (!op.IsDone)
            {
                onLoading?.Invoke(op.PercentComplete);
                await UniTask.Yield();
            }
            TPanel panel = op.Result.GetComponent<TPanel>();
            panel.Prepare();
            panel.PostInit();

            onLoaded?.Invoke();
            return panel;
        }

        public static TPanel Create<TPanel>(GameObject prefab) where TPanel : Panel
        {
            var op = Instantiate(prefab, PanelManager.Instance.transform);
            TPanel panel = op.GetComponent<TPanel>();
            panel.Prepare();
            panel.PostInit();
            return panel;
        }
        public void Register(Panel panel)
        {
            if (panels.Contains(panel)) return;
            panels.Add(panel);
        }
        public void Unregister(Panel panel)
        {
            if (!panels.Contains(panel)) return;
            panels.Remove(panel);
        }
        public void Init()
        {
            Instance = this;
            Transform holder = transform;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].Prepare();
                panels[i].PostInit();
            }
            for (int i = 0; i < holder.childCount; i++)
            {
                Panel panel = holder.GetChild(i).GetComponent<Panel>();
                if (panel == null) continue;
                try
                {
                    panel.Prepare();
                    panel.PostInit();
                    panels.Add(panel);
                }
                catch (System.Exception e) { GameUtility.GameUtility.LogError(panel.gameObject.name + " \n" + e); }
            }
        }
        public bool canBack = true;
        public void OnPanelShown(Panel panel)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            if (!stack.Contains(panel))
            {
                //GameUtility.GameUtility.Log("Show :" + panel.gameObject.name);
                stack.Push(panel);
            }
#endif
        }
        public void OnPanelHidden(Panel panel)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            if (stack.Count > 0 && stack.Peek() == panel)
            {
                GameUtility.GameUtility.Log("HIDE :" + panel.gameObject.name);
                stack.Pop();
            }
#endif
        }
        public Panel GetPanel(int index)
        {
            return panels[index];
        }

        public T GetPanel<T>() where T : Panel
        {
            var type = typeof(T);
            foreach (var panel in panels)
            {
                if (type == panel.GetType()) return (T)panel;
            }
            return null;
        }

        public void SetBack(bool v)
        {
            canBack = v;
        }

        [Button]
        public Panel GetLast()
        {
            if (panels.Count == 0) return null;
            return this.panels.Last();
        }

        public async UniTask CloseAll()
        {
            var list = new List<UniTask>();
            foreach (var panel in panels)
            {
                list.Add(panel.Close());
            }
            panels.Clear();
            await UniTask.WhenAll(list);
        }
    }
}
