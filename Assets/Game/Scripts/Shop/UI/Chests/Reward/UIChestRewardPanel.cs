using Assets.Game.Scripts.Utilities;
using com.mec;
using Cysharp.Threading.Tasks;
using GameUtility;
using Sirenix.OdinInspector;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIChestRewardPanel : UI.Panel
{
    [Header("Timing")]
    public UnityEvent onOpenPanel;
    public UnityEvent onOpenPanelChest_1;
    [Header("Thời gian delay lúc mở panel")]
    public float timeWait_EffectFirstTime;
    public float timeWait_EffectFirstTime1;
    public UnityEvent onActionAfterDelay;
    public UnityEvent onActionAfterDelay1;
    [Header("Thời gian delay lúc mở chest (thời gian show fx)")]
    public float timeWait_OpenChest;
    public float timeWait_OpenChest1;
    public UnityEvent onActionOpenChest;
    public UnityEvent onActionOpenChest1;
    public UnityEvent onActionCompleteChest;


    [SerializeField] private Image spriteChestBottomMask;
    [SerializeField] private Sprite bottomSilver;
    [SerializeField] private Sprite bottomGolden;
    [SerializeField] private Sprite bottomHero;

    [SerializeField] private RectTransform spawnPointScroller;
    [SerializeField] private UIActor uiChest;

    private List<FXChestScroller> m_Scrollers = new List<FXChestScroller>();
    [SerializeField]
    private UIIconText currencyReward;
    [SerializeField]
    private UIIconText otherReward;

    [SerializeField] private UiParticles.UiParticles appearRewardLeftBottom;
    [SerializeField] private UiParticles.UiParticles appearRewardRightBottom;

    private EChest chest;
    public List<LootParams> rewards;

    private ResourceData currencyData;
    private ResourceData otherSideData;
    public List<LootParams> listTarget;
    private List<Sprite> sprites = new List<Sprite>();
    private int numberOfLine = 1;

    private List<int> TargetIndex = new List<int>();

    public override void PostInit()
    {
        Sound.Controller.Instance.PauseMusic();
        currencyReward.gameObject.SetActive(false);
        otherReward.gameObject.SetActive(false);
    }


    public async void SetReward(EChest chest, List<LootParams> rewards)
    {

        switch (chest)
        {
            case EChest.Silver:
                uiChest.SetSkin("0");
                spriteChestBottomMask.sprite = bottomSilver;
                break;
            case EChest.Golden:
            case EChest.Golden10:
                uiChest.SetSkin("1");
                spriteChestBottomMask.sprite = bottomGolden;
                break;
            case EChest.Hero:
            case EChest.Hero10:
                uiChest.SetSkin("2");
                spriteChestBottomMask.sprite = bottomHero;
                break;
        }
        spriteChestBottomMask.SetNativeSize();

        canClose = false;
        this.chest = chest;
        this.rewards = rewards;
        currencyData = new ResourceData
        {
            Resource = EResource.Gold,
            Value = GetAmountResource(rewards, EResource.Gold, EResource.Gold).Value
        };

        numberOfLine = 1;

        switch (chest)
        {
            case EChest.Silver:
            case EChest.Golden:
            case EChest.Golden10:
                var equipmentAll = DataManager.Base.Equipment.Dictionary.Values;
                foreach (var equipment in equipmentAll)
                {
                    if (equipment.IdNum >= 0)
                    {
                        sprites.Add(ResourcesLoader.Instance.GetSprite(AtlasName.Equipment, $"{equipment.Id}"));
                    }
                }

                foreach (var rw in rewards)
                {
                    if (rw.Data is EquipmentData)
                    {
                        foreach (var equipment in equipmentAll)
                        {
                            if (equipment.IdNum == (((EquipmentData)rw.Data).Id))
                            {
                                TargetIndex.Add(equipment.IdNum);
                            }
                        }
                    }
                }

                if (chest == EChest.Golden10)
                {
                    numberOfLine = 10 / 2;
                }
                otherSideData = new ResourceData { Value = GetAmountResource(rewards, EResource.MainWpFragment, EResource.BootFragment).Value, Resource = EResource.EquipmentRdFragment };
                break;
            case EChest.Hero:
            case EChest.Hero10:
                for (var i = EResource.NormalHero; i <= EResource.EvilHero; i++)
                {
                    sprites.Add(ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{i}"));
                }
                if (chest == EChest.Hero10)
                {
                    numberOfLine = 10 / 2;
                }

                foreach (var rw in rewards)
                {
                    if (rw.Data is ResourceData)
                    {
                        var rs = rw.Data as ResourceData;
                        int id = 0;
                        for (var i = EResource.NormalHero; i <= EResource.EvilHero; i++)
                        {
                            if (rs.Resource == i)
                            {
                                TargetIndex.Add(id);
                                break;
                            }
                            id++;
                        }
                    }
                }

                otherSideData = new ResourceData { Value = GetAmountResource(rewards, EResource.BaseStone, EResource.LightStone).Value, Resource = EResource.HeroStoneRdFragment };
                break;
        }
        currencyReward.Set(currencyData);
        otherReward.Set(otherSideData);
        var target = GetTargets(rewards, chest >= EChest.Hero ? ELootType.HeroFragment : ELootType.Equipment);

        if (chest >= EChest.Hero)
        {
        }
        else
        {
            target = target.OrderByDescending(t => (t.Data as EquipmentData).Rarity).Reverse().ToList();
        }

        if (numberOfLine == 1)
        {
            onOpenPanelChest_1?.Invoke();
        }
        else
        {
            onOpenPanel?.Invoke();
        }

        if (numberOfLine == 10)
        {
            numberOfLine = 5;
        }
        uiChest.SetTimeScale(0);

        await SpawnScroller(numberOfLine, target);

        Timing.RunCoroutine(_TimingShowChest());
    }

    private IEnumerator<float> _TimingShowChest()
    {
        if (numberOfLine > 1)
        {
            yield return Timing.WaitForSeconds(timeWait_EffectFirstTime);
            onActionAfterDelay?.Invoke();

            yield return Timing.WaitForSeconds(timeWait_OpenChest);
            onActionOpenChest?.Invoke();
        }
        else
        {
            yield return Timing.WaitForSeconds(timeWait_EffectFirstTime1);
            onActionAfterDelay1?.Invoke();

            yield return Timing.WaitForSeconds(timeWait_OpenChest1);
            onActionOpenChest1?.Invoke();
        }
        uiChest.SetTimeScale(1);

        uiChest.SetAnimation(0, "open", false, onEvent: OnEventTracking);
        Timing.RunCoroutine(_AppearEffectBottom());
    }

    private IEnumerator<float> _AppearEffectBottom()
    {
        yield return Timing.WaitForSeconds(0.5f);
        appearRewardLeftBottom.ParticleSystem.Play();
        currencyReward.gameObject.SetActive(true);
        yield return Timing.WaitForSeconds(1f);

        appearRewardRightBottom.ParticleSystem.Play();
        otherReward.gameObject.SetActive(true);
    }

    private void OnEventTracking(TrackEntry track, Spine.Event data)
    {
        if (data.Data.Name == "attack_tracking")
        {
            StartScroller();
            Timing.RunCoroutine(_RunIncreaseReward());
        }
    }

    private IEnumerator<float> _RunIncreaseReward()
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

        while (timeCur < totalTimeAnimation)
        {
            left = (int)Mathf.Lerp(0, leftTarget, timeCur / totalTimeAnimation);
            right = (int)Mathf.Lerp(0, rightTarget, timeCur / totalTimeAnimation);

            currencyReward.SetAmountFormat(left.TruncateValue());
            otherReward.SetAmountFormat(right.TruncateValue());

            timeCur += Time.deltaTime;
            yield return Timing.DeltaTime;
        }

        currencyReward.SetAmountFormat(leftTarget.TruncateValue());
        otherReward.SetAmountFormat(rightTarget.TruncateValue());

        onActionCompleteChest?.Invoke();
    }

    private void StartScroller()
    {
        Timing.RunCoroutine(_Scroller(), gameObject);
    }
    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
    }

    [Header("Reward 1 Chest")]
    [Header("Thời gian delay dừng lại")]
    public float OneChest_timeDelayStop;
    public float OneChest_delaySpawn;
    public float OneChest_FXChest;

    /*
    
    0
    1
    2
    3
    4
    0 4 1 3 2
    */
    [Header("Reward 10 Chest")]

    [ShowInInspector]
    [Header("Thứ tự show")]
    public List<int> _scrollStt = new List<int>
    {
       0, 4, 1, 3, 2
    };

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

    private float totalTimeAnimation = 0;
    [SerializeField] private float timeFXChest = 3;

    // Time delay for last time spawn
    private float timeDelayForLastTimeSpawn;
    private IEnumerator<float> _Scroller()
    {
        timeDelayForLastTimeSpawn = 0;
        yield return Timing.DeltaTime;
        if (numberOfLine == 1)
        {
            totalTimeAnimation = OneChest_FXChest + 1f + OneChest_timeDelayStop;
            yield return Timing.WaitForSeconds(OneChest_delaySpawn);
            m_Scrollers[0].SetTimeout(OneChest_FXChest);
            m_Scrollers[0].StartScroll();
        }
        else
        {
            totalTimeAnimation = timeFXChest + 1f + _timeDelayStop.Max(t => t.Value);
            timeDelayForLastTimeSpawn = 1f + _delaySpawn.Max(t => t.Value);

            foreach (var i in _timeDelayStop)
            {
                var timeExtra = i.Value;
                foreach (var id in i.Key)
                {
                    var scroll = m_Scrollers[id];
                    scroll.SetTimeout(timeFXChest + timeExtra);
                }
            }

            foreach (var t in _delaySpawn)
            {
                foreach (var index in t.Key)
                {
                    m_Scrollers[index]
                        .SetTimeout(m_Scrollers[index].TimeOut + _delaySpawn.Max(t=>t.Value) - t.Value);
                    Timing.RunCoroutine(_StartScroll(index, t.Value), gameObject);
                }
                totalTimeAnimation += t.Value;
            }
        }
    }

    private IEnumerator<float> _StartScroll(int index, float value)
    {
        yield return Timing.WaitForSeconds(value);
        m_Scrollers[index].StartScroll();
    }

    [Button]
    private async UniTask SpawnScroller(int numberOfLine, List<LootParams> target)
    {
        if (numberOfLine == 1)
        {
            await SpawnSroller(0).ContinueWith(scroll =>
            {
                scroll.SetData(chest, target[0]);
                scroll.SetTargetIndex(TargetIndex[0]);
                scroll.SetListSprite(sprites);
                scroll.SetPositionInLine(2);
                scroll.gameObject.SetActive(false);
                scroll.SetTimeout(timeFXChest);
            });
        }
        else
        {
            float angleIncrease = 25;
            float angle = 0;
            for (int i = -numberOfLine/2; i <= numberOfLine/2; i++)
            {
                var scroll = await SpawnSroller(angle + angleIncrease * i);
                    
                scroll.gameObject.SetActive(false);
                //angle += angleIncrease;
            }
            int index = 0;
            for (int i = 0; i < numberOfLine; i++)
            {
                m_Scrollers[i].SetPositionInLine((i % 2 == 0 ? 1 : 0));
                for (int j = 0; j < 2; j++)
                {
                    var scroll = m_Scrollers[_scrollStt[i]];
                    scroll.SetData(chest, target[index]);
                    scroll.SetListSprite(sprites);
                    scroll.SetTargetIndex(TargetIndex[index]);
                    scroll.SetTimeout(timeFXChest);
                    index++;
                }
            } 
            foreach (var i in _timeDelayStop)
            {
                var timeExtra = i.Value + timeDelayForLastTimeSpawn;
                foreach (var id in i.Key)
                {
                    var scroll = m_Scrollers[id];
                    scroll.SetTimeout(timeFXChest + timeExtra);
                }
            }
        }
    }

    private async UniTask<FXChestScroller> SpawnSroller(float angle)
    {
        if (numberOfLine == 1)
        {
            var scroller = await ResourcesLoader.Instance.GetAsync<FXChestScroller>(AddressableName.UIFXChestScroller_1, spawnPointScroller);
            scroller.transform.localPosition = Vector3.zero;
            scroller.transform.eulerAngles = Vector3.forward * angle;

            m_Scrollers.Add(scroller);
            return scroller;
        }
        else
        {
            var scroller = await ResourcesLoader.Instance.GetAsync<FXChestScroller2>(AddressableName.UIFXChestScroller_2, spawnPointScroller);
            scroller.transform.localPosition = Vector3.zero;
            scroller.transform.eulerAngles = Vector3.forward * angle;

            m_Scrollers.Add(scroller);
            return scroller;
        }
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
    public List<LootParams> GetTargets(List<LootParams> rewards, ELootType lootType)
    {
        var res = new List<LootParams>();

        foreach (var rw in rewards)
        {
            if (rw.Type == lootType)
            {
                res.Add(rw);
            }
        }

        return res;
    }

    private bool canClose = false;
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
        canClose = true;
        Close();
    }
    public override void Close()
    {
        if (!canClose) return;
        Sound.Controller.Instance.ContinueMusic();
        base.Close();
    }
}