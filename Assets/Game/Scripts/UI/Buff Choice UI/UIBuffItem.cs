using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreMountains.Tools;
using Mosframe;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuffItem : UIBuffItemBase
{
    [SerializeField] private DynamicVScrollView scrollViewJackpot;
    [SerializeField] private TextMeshProUGUI buffDescriptionTxt;

    [SerializeField] private GameObject mainObject;
    [SerializeField] private GameObject scrollObject;

    [SerializeField]
    private bool sendEvent = true;

    private bool CanPick = false;

    protected List<int> idBuffJackpot = new List<int>();


    public List<int> IdDataBuff => idBuffJackpot;
    public override void SetEntity(BuffEntity entity)
    {
        base.SetEntity(entity);
    }

    private async UniTask PresentJackpotBuff(float delay)
    {
        CanPick = false;
        scrollObject.SetActive(true);
        mainObject.SetActive(false);
        var targetId = this.buffEntity.Id;

        idBuffJackpot.Clear();
        var allBuff = DataManager.Base.Buff.Dictionary.Values.ToList();
        if (buffType == EPickBuffType.Normal)
        {
            allBuff.RemoveAll(t => t.PickType != (base.buffType));
        }
        int countOriginBuff = allBuff.Count;

        for (int i = 0; i < allBuff.Count; i++)
        {
            IdDataBuff.Add(allBuff[i].Id);
        }
        IdDataBuff.MMShuffle();

        int index = IdDataBuff.FindLastIndex(t => t == targetId);
        IdDataBuff.MMSwap(index, IdDataBuff.Count - 1);

        index = IdDataBuff.FindLastIndex(t => t == targetId);
        scrollViewJackpot.init(IdDataBuff.Count);
        scrollViewJackpot.refresh();
        scrollViewJackpot.SetPosition(0);

        await UniTask.Delay(200);
        await scrollViewJackpot.scroll(index, delay, ()=>
        {
            scrollViewJackpot.onScroll.RemoveAllListeners();
            scrollObject.SetActive(false);
            mainObject.SetActive(true);
            CanPick = true;
        }).AsyncWaitForCompletion();
    }

    public override void PrepareAnimation()
    {
        base.PrepareAnimation();
        canvasGroup.alpha = 0;

        //var eff = await ResourcesLoader.Instance.GetAsync<ParticleSystem>(AddressableName.VFX_BUFF_START);
        //if (eff != null)
        //{
        //    eff.transform.SetParent(transform);
        //    eff.Play();
        //}
    }
    public async UniTask PlayAnimation(float stopAfter = 1.0f)
    {
        DOTween.Kill(gameObject);
        float durationAnimation = 0.3f;
        Sequence seq = DOTween.Sequence(gameObject);
        seq.Join(canvasGroup.DOFade(1, durationAnimation).From(0))
            .Join(rectTransform.DOScale(Vector3.one, durationAnimation).From(Vector3.one * 1.5f).SetEase(Ease.OutBack));

        Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Scroll_Buff);

        await PresentJackpotBuff(stopAfter);
        Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Scroll_Buff_End);
    }

    public void DisappearAnimation()
    {
        DOTween.Kill(gameObject);
        float durationAnimation = 0.3f;
        Sequence seq = DOTween.Sequence(gameObject);
        seq.Join(canvasGroup.DOFade(0, durationAnimation).From(1))
            .Join(rectTransform.DOScale(Vector3.one * 1.5f, durationAnimation).From(Vector3.one).SetEase(Ease.InBack));
    }

    public async void BuffOnClicked()
    {
        if (CanPick == false) return;
        Debug.Log("Pick Buff: " + buffEntity.Type);
        Messenger.Broadcast(EventKey.CastBuffToMainPlayer, buffEntity.Type);
        if (sendEvent)
        {
            Messenger.Broadcast(EventKey.PickBuffDone, buffType);
        }

        //var eff = await ResourcesLoader.Instance.GetAsync<ParticleSystem>(AddressableName.VFX_BUFF_PICK);
        //if (eff != null)
        //{
        //    eff.transform.SetParent(transform);
        //    eff.Play();
        //}
    }
    public override void SetDescription()
    {
        base.SetDescription();
        if (buffDescriptionTxt != null)
        {
            buffDescriptionTxt.text = buffEntity.GetDescription();
        }
    }
}