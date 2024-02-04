using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mosframe;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FXChestScroller : MonoBehaviour
{
    public int Id = 0;
    public float alphaColor = 0.75f;

    [SerializeField] protected ParticleSystem normalEffectEnd;
    [SerializeField] protected ParticleSystem epicEffectEnd;


    [SerializeField] protected Color heroFragmentColor;
    [SerializeField] protected Color commonColor;
    [SerializeField] protected Color unCommonColor;
    [SerializeField] protected Color rareColor;
    [SerializeField] protected Color epicColor;
    [SerializeField] protected Color legendaryColor;
    [SerializeField] protected Color ultimateColor;

    [SerializeField] protected UnityEvent onCompleted;

    [SerializeField] protected UISetColorImage[] setColorsImage;
    [SerializeField] protected DynamicVScrollView _scrollRect;
    [SerializeField] protected Vector2 direction = Vector2.up;
    [SerializeField] protected float velocity = 0;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected Image panelBg;
    [SerializeField] protected ParticleSystem panelEffect;


    protected System.Action onStopCallback;
    protected float TimeStop;
    protected EChest chestType;
    [ShowInInspector]
    protected List<LootParams> target = new List<LootParams>();
    protected List<Sprite> sprites = new List<Sprite>();
    public List<int> TargetIndex;
    public List<Sprite> Sprites => sprites;
    protected float time = 0;
    protected bool stop;
    public bool Stopped => stop;

    public int CurrentCycle = 0;
    public float TimeOut => TimeStop;
    public bool IsRunning { set; get; } = false;

    private void OnEnable()
    {
        panelEffect.Stop();
        stop = false;
        canvasGroup.alpha = 0;
    }

    public virtual void StartScroll()
    {
        if (IsRunning) return;
        IsRunning = true;
        gameObject.SetActive(true);
        Timing.RunCoroutine(_Scrolling(), gameObject);
    }

    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
    }

    public virtual void SetTimeout(float timeout)
    {
        TimeStop = timeout;
    }

    public virtual void SetTargetIndex(int index)
    {
        TargetIndex.Insert(0, index);
    }

    public virtual void SetData(EChest chest, LootParams target)
    {
        this.chestType = chest;
        this.target.Insert(0, target);
        Setup();
    }

    public virtual void Setup() { }
    float yTarget = 0;
    protected List<int> PositionInLine = new List<int>();

    public virtual void SetPositionInLine(int positionInLine)
    {
        this.PositionInLine.Add(positionInLine);
    }

    protected virtual IEnumerator<float> _Scrolling()
    {
        time = 0;
        stop = false;
        yield return Timing.WaitForSeconds(0.25f);
        _scrollRect.Content.anchoredPosition = Vector3.zero;
        canvasGroup.alpha = 1;
        _scrollRect.GetComponent<RectTransform>().DOSizeDelta(new Vector2(125, 1000), 0.2f).From(new Vector2(125, 100));
        bool isStop = false;
        while (true)
        {
            if (time > TimeStop && !isStop)
            {
                break;
            }
            _scrollRect.Content.anchoredPosition += direction * velocity * Time.deltaTime;
            time += Time.deltaTime;
            yield return Timing.DeltaTime;
        }
        panelEffect.Play();
        float dur = 1;
        while (dur > 0)
        {
            _scrollRect.Content.anchoredPosition += direction * velocity * Time.deltaTime;
            dur -= Time.deltaTime;
            yield return Timing.DeltaTime;
        }
        _scrollRect.scrollByItemIndex(Sprites.Count * 2 + TargetIndex[0] - (1 + PositionInLine[0]));
        _scrollRect.refresh();
        OnScrollTo();
        onStopCallback?.Invoke();

        yield return Timing.WaitForSeconds(2);
        stop = true;
    }

    public virtual void SetListSprite(List<Sprite> sprites)
    {
        this.sprites = sprites;
    }

    int targetScroll;
    protected Color colorBgPanel;

    public virtual void SetColor(Color c)
    {
        colorBgPanel = c;
        colorBgPanel.a = alphaColor;
    }

    protected virtual void OnScrollTo()
    {
        onCompleted?.Invoke();
        var type = target[0].Type;
        if (type == ELootType.HeroFragment || type == ELootType.Resource || type == ELootType.HeroStone)
        {
            normalEffectEnd.Play();
            SetColor(heroFragmentColor);
        }
        else
        {
            var eq = (target[0].Data as EquipmentData);
            if (eq != null)
            {
                Color c = commonColor;
                switch (eq.Rarity)
                {
                    case ERarity.UnCommon:
                        c = unCommonColor;
                        break;
                    case ERarity.Rare:
                        c = rareColor;
                        break;
                    case ERarity.Epic:
                        c = epicColor;
                        break;
                    case ERarity.Legendary:
                        c = legendaryColor;
                        break;
                    case ERarity.Ultimate:
                        c = ultimateColor;
                        break;
                }

                if (eq.Rarity == ERarity.Epic)
                {
                    epicEffectEnd.Play();
                }
                else
                {
                    normalEffectEnd.Play();
                }
                c.a = alphaColor;
                SetColor(c);
            }
            else
            {
                normalEffectEnd.Play();
                SetColor(heroFragmentColor);
            }
        }
        var item = _scrollRect.getItemIndex(Sprites.Count * 2 + TargetIndex[0]) as FXChestRewardItem;

        item.SetColor(target[0]);

        foreach (var im in setColorsImage)
        {
            im.SetColor(colorBgPanel);
        }
        _scrollRect.SetActiveFalseAllExcept(Sprites.Count * 2 + TargetIndex[0]);
    }

    public LootParams GetTarget(int v)
    {
        return target[v];
    }

    public virtual async void StopNow()
    {
        _scrollRect.Content.anchoredPosition = Vector3.zero;
        canvasGroup.alpha = 1;
        _scrollRect.GetComponent<RectTransform>().DOSizeDelta(new Vector2(125, 1000), 0.05f).From(new Vector2(125, 100));
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        Timing.KillCoroutines(gameObject);
        _scrollRect.scrollByItemIndex(Sprites.Count * 2 + TargetIndex[0] - (1 + PositionInLine[0]));
        _scrollRect.refresh();
        OnScrollTo();
        onStopCallback?.Invoke();

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        stop = true;
    }
}