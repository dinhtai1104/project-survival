using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace UI
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
        public static void Create(Type type,System.Action<Panel> onLoaded)
        {
            Create(type.ToString(), onLoaded);
        }
        public static void Create(string type, System.Action<Panel> onLoaded)
        {
            CreateAsync(type).ContinueWith(onLoaded).Forget();
        }
        public static async UniTask<Panel> CreateAsync(string type)
        {
            AsyncOperationHandle<GameObject> op =  Addressables.InstantiateAsync(type, PanelManager.Instance.transform);
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

        public static TPanel Create<TPanel>(GameObject prefab) where TPanel : Panel
        {
            var op = Instantiate(prefab, PanelManager.Instance.transform);
            TPanel panel = op.GetComponent<TPanel>();
            panel.Prepare();
            panel.PostInit();
            return panel;
        }
        public async void Register(Panel panel)
        {
            if (panels.Contains(panel)) return;
            panels.Add(panel);
        }
        public void DeRegister(Panel panel)
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
//#if UNITY_ANDROID || UNITY_EDITOR
//        private void Update()
//        {
//            if (canBack &&Input.GetKeyDown(KeyCode.Escape) &&stack!=null && stack.Count > 0 )
//            {
//                while (stack.Count>0 &&(stack.Peek()==null ||!stack.Peek().gameObject.activeSelf ) && (stack.Peek()==null||( stack.Peek()!=null&&!stack.Peek().isPersistant)))
//                {
//                    stack.Pop();
//                }
//                if (stack.Count > 0 && stack.Peek()!=null)
//                {
//                    //GameUtility.GameUtility.Log("back :" + stack.Peek().gameObject.name);
//                    if (stack.Peek().isPersistant)
//                    {
//                        stack.Peek().OnBack();
//                    }
//                    else
//                    {
//                        stack.Pop().OnBack();
//                    }
//                }
//            }
//        }
//#endif
      
        public void OnPanelShown(UI.Panel panel)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            if (!stack.Contains(panel))
            {
                //GameUtility.GameUtility.Log("Show :" + panel.gameObject.name);
                stack.Push(panel);
            }
#endif
        }
        public void OnPanelHidden(UI.Panel panel)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            if (stack.Count>0 &&stack.Peek() == panel)
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

        public T GetPanel<T>() where T: Panel
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


        public static async UniTask<UINoticePanel> ShowNotice(string message, bool translate = false, ENotice noticeType = ENotice.YesNo)
        {
            if (translate)
            {
                message = I2Localize.GetLocalize(message);
            }
            var notice = await UI.PanelManager.CreateAsync<UINoticePanel>(AddressableName.UINoticePanel);
            notice.Show();
            notice.SetText(message, noticeType: noticeType);
            return notice;
        }

        public static async UniTask<UIRewardPanel> ShowRewards(List<LootParams> lootParams, bool isLoot = true, int multiply = 1)
        {
            var uiReward = await PanelManager.CreateAsync<UIRewardPanel>(AddressableName.UIRewardPanel);

            var loot = lootParams.Refactor(multiply);
            uiReward.Show(loot);

            if (isLoot)
            {
                foreach (var data in loot)
                {
                    data.Data.Loot();
                }
            }
            return uiReward;
        }
        public static async UniTask<UIRewardPanel> ShowRewards(LootParams lootParams, bool isLoot = true)
        {
            return await ShowRewards(new List<LootParams> { lootParams }, isLoot);
        }

        [Button]
        public Panel GetLast()
        {
            if (panels.Count == 0) return null;
            return this.panels.Last();
        }
    }
}