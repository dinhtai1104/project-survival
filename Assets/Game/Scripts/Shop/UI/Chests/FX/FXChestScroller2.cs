using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mosframe;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FXChestScroller2 : FXChestScroller
{ 
    protected override IEnumerator<float> _Scrolling()
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
        _scrollRect.scrollByItemIndex(Sprites.Count * 2);
        _scrollRect.refresh();
        OnScrollTo();
        onStopCallback?.Invoke();

        yield return Timing.WaitForSeconds(2);
        stop = true;
    }
    protected override void OnScrollTo()
    {
        onCompleted?.Invoke();
        _scrollRect.SetActiveFalseAllExcept(Sprites.Count * 2 + 1 + PositionInLine[0], Sprites.Count * 2 + 2 + PositionInLine[0]);
        var item = _scrollRect.getItemIndex(Sprites.Count * 2 + 1 + PositionInLine[0]) as FXChestRewardItem;
        var item2 = _scrollRect.getItemIndex(Sprites.Count * 2 + 2 + PositionInLine[0]) as FXChestRewardItem;

        item.SetIndex(TargetIndex[0]);
        item2.SetIndex(TargetIndex[1]);

        SetColorItem(target[1], item2);
        SetColorItem(target[0], item);

        //foreach (var im in setColorsImage)
        //{
        //    im.SetColor(colorBgPanel);
        //}
    }

    public async override void StopNow()
    {
        _scrollRect.Content.anchoredPosition = Vector3.zero;
        canvasGroup.alpha = 1;
        _scrollRect.GetComponent<RectTransform>().DOSizeDelta(new Vector2(125, 1000), 0.05f).From(new Vector2(125, 100));
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        Timing.KillCoroutines(gameObject);
        _scrollRect.scrollByItemIndex(Sprites.Count * 2);
        _scrollRect.refresh();
        OnScrollTo();
        onStopCallback?.Invoke();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        stop = true;
    }

    public void SetColorItem(LootParams target, FXChestRewardItem fx)
    {
        var Color = heroFragmentColor;
        if (target.Type == ELootType.HeroFragment)
        {
            normalEffectEnd.Play();
            Color = heroFragmentColor;
        }
        else
        {
            var eq = (target.Data as EquipmentData);
            if (eq != null)
            {
                Color = commonColor;
                switch (eq.Rarity)
                {
                    case ERarity.UnCommon:
                        Color = unCommonColor;
                        break;
                    case ERarity.Rare:
                        Color = rareColor;
                        break;
                    case ERarity.Epic:
                        Color = epicColor;
                        break;
                    case ERarity.Legendary:
                        Color = legendaryColor;
                        break;
                    case ERarity.Ultimate:
                        Color = ultimateColor;
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
            }
            else
            {
                Color = legendaryColor;
                epicEffectEnd.Play();
            }
        }
        fx.SetColor(target);
        
        colorBgPanel = Color;
        foreach (var im in setColorsImage)
        {
            colorBgPanel.a = alphaColor;
            im.SetColor(colorBgPanel);
        }
    }
}