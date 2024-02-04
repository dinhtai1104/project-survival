using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using com.mec;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Linq;
using Spine;
using System;
using DG.Tweening;
using Castle.Core.Internal;

public abstract class UIChestRewardBasePanel : UI.Panel
{
    protected EChest chest;
    public int NumberOfLine = 1;
    public int NumberItemOfOneLine = 1;
    public float offsetAngle = 20;
    public UIActor uiChest;
    public GameObject maskObject;
    [SerializeField] private UiParticles.UiParticles appearRewardLeftBottom;
    [SerializeField] private UiParticles.UiParticles appearRewardRightBottom;
    [SerializeField] protected GameObject buttonSkip;
    [Header("Event invoke when open panel")]
    public UnityEvent onOpenPanelEvent;
    [Header("Event invoke after wait time after open panel")]
    public UnityEvent onCompleteWaitForOpenChestEvent;
    [Header("Event invoke when open chest")]
    public UnityEvent onStartOpenChestEvent;
    [Header("Event invoke when complete open fx chest")]
    public UnityEvent onCompleteOpenChestEvent;

    [Header("Timing")]
    public float TimeChestFX = 3;
    public float timeWait_EffectFirstTime;
    public float timeWait_OpenChest;
    public List<LootParams> rewards;
    public List<LootParams> targets;

    protected ResourceData currencyData;
    protected ResourceData otherSideData;


    [Header("Reward Bottom")]
    [SerializeField] protected UIIconText currencyReward;
    [SerializeField] protected UIIconText otherReward;


    [Header("Thứ tự show")]
    public List<int> _scrollStt;

    [System.Serializable]
    public class PairListFloat
    {
        public List<int> Key;
        public float Value;
    }

    [ShowInInspector]
    [Header("Thời gian delay dừng lại")]
    public List<PairListFloat> _timeDelayStop = new List<PairListFloat>
    {
        new PairListFloat{ Key = new List<int> {0, 4}, Value = 0},
        new PairListFloat{ Key = new List<int> {1, 3}, Value = 2},
        new PairListFloat{ Key = new List<int> {2}, Value = 4},
    };

    [ShowInInspector]
    [Header("Thời gian delay xuất hiện")]
    public List<PairListFloat> _delaySpawn = new List<PairListFloat>
    {
        new PairListFloat{ Key = new List<int>() {1, 2, 3}, Value = 0 },
        new PairListFloat{ Key = new List<int>() {0, 4}, Value = 3 },
    };

    [Header("Scroller View")]
    public string AddressableFXChestScroller = AddressableName.UIFXChestScroller_1;
    public RectTransform spawnPointScroller;
    protected List<FXChestScroller> m_Scrollers = new List<FXChestScroller>();
    protected List<Sprite> sprites = new List<Sprite>();
    protected List<int> TargetIndex = new List<int>();
    protected float totalAnimationTime;
    public List<List<LootParams>> rewardsRaw;

    private void Awake()
    {
        buttonSkip.SetActive(false);
    }

    public void SetRewardRaw(List<List<LootParams>> rewards)
    {
        this.rewardsRaw = rewards;
    }
    public async void SetReward(EChest chest, List<LootParams> rewards)
    {
        this.chest = chest;
        uiChest.SetTimeScale(0);
        this.rewards = rewards;

        currencyData = new ResourceData
        {
            Resource = EResource.Gold,
            Value = GetAmountResource(rewards, EResource.Gold, EResource.Gold).Value
        };

        await SpawnScroller(NumberOfLine);
        SetupData();

        currencyReward.Set(currencyData);
        otherReward.Set(otherSideData);

        onOpenPanelEvent?.Invoke();

        Timing.RunCoroutine(_TimingShowChest(), gameObject);
        Timing.RunCoroutine(_WaitForSkipButton(), gameObject);
    }

    protected virtual IEnumerator<float> _WaitForSkipButton()
    {
        yield return Timing.WaitForSeconds(3);
        var save = DataManager.Save.Chest.Saves[chest];
        if (save.OpenTime >= 2)
        {
            buttonSkip.SetActive(true);
        }
    }

    protected abstract void SetupData();

    private IEnumerator<float> _TimingShowChest()
    {
        yield return Timing.WaitForSeconds(timeWait_EffectFirstTime);
        Sound.Controller.Instance.Play(GetComponentInChildren<AudioSource>(), GetComponentInChildren<AudioSource>().clip);
        onCompleteWaitForOpenChestEvent?.Invoke();

        yield return Timing.WaitForSeconds(timeWait_OpenChest);
        onStartOpenChestEvent?.Invoke();

        uiChest.SetTimeScale(1);
        uiChest.SetAnimation(0, "open", false, onEvent: OnEventTracking, onComplete: OnCompleteTracking);
        uiChest.AddAnimation(0, "open_idle", true, 0.1f);
        Timing.RunCoroutine(_AppearEffectBottom());
    }

    private void OnCompleteTracking(TrackEntry obj)
    {
        maskObject.SetActive(true);
    }

    private void OnEventTracking(TrackEntry track, Spine.Event data)
    {
        if (data.Data.Name == "attack_tracking")
        {
            StartScroller();
        }
    }

    public virtual void StartScroller()
    {
        Timing.RunCoroutine(_Scroller(), gameObject);
    }

    public override void PostInit()
    {
        Sound.Controller.Instance.PauseMusic();
        currencyReward.gameObject.SetActive(false);
        otherReward.gameObject.SetActive(false);
    }

    protected virtual IEnumerator<float> _Scroller()
    {
        yield return Timing.DeltaTime;
        totalAnimationTime = TimeChestFX + 1f + _timeDelayStop.Max(t => t.Value);

        foreach (var i in _timeDelayStop)
        {
            var timeExtra = i.Value;
            foreach (var id in i.Key)
            {
                var scroll = m_Scrollers[id];
                scroll.SetTimeout(TimeChestFX + timeExtra);
            }
        }

        foreach (var t in _delaySpawn)
        {
            foreach (var index in t.Key)
            {
                m_Scrollers[index]
                    .SetTimeout(m_Scrollers[index].TimeOut + _delaySpawn.Max(t => t.Value) - t.Value);
                Timing.RunCoroutine(_StartScroll(index, t.Value), gameObject);
            }
            totalAnimationTime += t.Value;
        }
        Timing.RunCoroutine(_RunIncreaseReward());
    }

    protected async UniTask<FXChestScroller> SpawnSroller(float angle)
    {
        var scroller = await ResourcesLoader.Instance.GetAsync<FXChestScroller>(AddressableFXChestScroller, spawnPointScroller);
        scroller.transform.localPosition = Vector3.zero;
        scroller.transform.eulerAngles = Vector3.forward * angle;

        m_Scrollers.Add(scroller);
        return scroller;
    }

    protected IEnumerator<float> _RunIncreaseReward()
    {
        currencyReward.SetAmountFormat(0.TruncateValue());
        otherReward.SetAmountFormat(0.TruncateValue());
        yield return Timing.DeltaTime;
        yield return Timing.DeltaTime;
        float timeCur = 0;
        int left = 0;
        int right = 0;
        int leftTarget = (int)currencyData.Value;
        int rightTarget = (int)otherSideData.Value;
        float timePunchScale = 0.1f;
        float timePunch = 0;

        while (timeCur < totalAnimationTime)
        {
            left = (int)Mathf.Lerp(0, leftTarget, timeCur / totalAnimationTime);
            right = (int)Mathf.Lerp(0, rightTarget, timeCur / totalAnimationTime);

            currencyReward.SetAmountFormat(left.TruncateValue());
            otherReward.SetAmountFormat(right.TruncateValue());

            if (timePunch >= timePunchScale)
            {
                currencyReward.transform.DOShakeScale(0.1f, 0.1f);
                otherReward.transform.DOShakeScale(0.1f, 0.1f);
                timePunch = 0;
            }

            timePunch += Time.deltaTime;
            timeCur += Time.deltaTime;
            yield return Timing.DeltaTime;
        }

        currencyReward.transform.DOScale(Vector3.one * 1.6f, 0.15f).OnComplete(()=>
        {
            currencyReward.transform.DOScale(Vector3.one * 1, 0.1f).SetEase(Ease.OutCubic);
        });
        otherReward.transform.DOScale(Vector3.one * 1.6f, 0.15f).OnComplete(()=>
        {
            otherReward.transform.DOScale(Vector3.one * 1, 0.1f).SetEase(Ease.OutCubic);
        });

        currencyReward.SetAmountFormat(leftTarget.TruncateValue());
        otherReward.SetAmountFormat(rightTarget.TruncateValue());

        uiChest.SetTimeScale(0);

        onCompleteOpenChestEvent?.Invoke();
    }

    protected virtual void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
    }

    protected IEnumerator<float> _StartScroll(int index, float delay)
    {
        yield return Timing.WaitForSeconds(delay);
        m_Scrollers[index].StartScroll();
    }

    [Button]
    protected async UniTask SpawnScroller(int numberOfLine)
    {
        if (numberOfLine == 1)
        {
            var scroll = await SpawnSroller(0);
            scroll.gameObject.SetActive(false);
        }
        else
        {
            float angleIncrease = offsetAngle;
            float angle = 0;
            for (int i = -numberOfLine / 2; i <= numberOfLine / 2; i++)
            {
                var scroll = await SpawnSroller(angle + angleIncrease * i);
                scroll.gameObject.SetActive(false);
            }
        }
    }


    public void SkipChestRewardOnClicked()
    {
        buttonSkip.SetActive(false);
        StopNow();
    }

    private async void StopNow()
    {
        totalAnimationTime = 0;
        onCompleteOpenChestEvent?.Invoke();
        GetComponentInChildren<AudioSource>()?.Stop();

        foreach (var sc in m_Scrollers)
        {
            if (!sc.IsRunning)
            {
                sc.StartScroll();
            }
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));

        foreach (var sc in m_Scrollers)
        {
            sc.StopNow();
        }
    }

    //EXTENSION
    #region EXTENSION
    private IEnumerator<float> _AppearEffectBottom()
    {
        yield return Timing.WaitForSeconds(0.5f);
        appearRewardLeftBottom.ParticleSystem.Play();
        currencyReward.gameObject.SetActive(true);
        yield return Timing.WaitForSeconds(1f);

        appearRewardRightBottom.ParticleSystem.Play();
        otherReward.gameObject.SetActive(true);
    }

    public List<LootParams> GetTargets(List<LootParams> rewards, params ELootType[] lootType)
    {
        var res = new List<LootParams>();

        foreach (var rw in rewards)
        {
            if (lootType.FindAll(t=> t == rw.Type).ToList().Count > 0)
            {
                res.Add(rw);
            }
        }

        return res;
    }

    public ResourceData GetAmountResource(List<LootParams> rewards, EResource min, EResource max)
    {
        var res = new ResourceData();

        foreach (var reward in rewards)
        {
            var data = reward.Data;
            if (data is ResourceData)
            {
                var r = (ResourceData)data;
                var type = r.Resource;
                if (type >= min && type <= max)
                {
                    res.Value += r.Value;
                }
            }
        }
        return res;
    }
    public async UniTask Stop()
    {
        await UniTask.Delay(1000);
        await UniTask.WaitUntil(() =>
        {
            var rs = true;
            foreach (var sc in m_Scrollers)
            {
                if (sc.Stopped == false) return false;
            }
            return rs;
        });
        Sound.Controller.Instance.Stop(GetComponentInChildren<AudioSource>());
        await UniTask.Delay(1000);

        Close();
    }

    public override void Close()
    {
        Sound.Controller.Instance.ContinueMusic();
        base.Close();
    }
    #endregion
}
