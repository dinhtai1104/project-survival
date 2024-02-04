using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Subscription.Services;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class UIAdOfferPanel : UI.Panel
{
    [SerializeField] private EPickBuffType _typeBuff;
    [SerializeField] private UIBuffItem[] _buffItems;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject offerBtn,boxObj,closeBtn;
    [SerializeField] private GameObject adObj;
    bool isBusy = false;
    public override void PostInit()
    {
    }

    public override void Show()
    {
        GameUIPanel.Instance.inputController.SetActive(false);
        base.Show();

        var subService = Architecture.Get<AdService>();
        var bpService = Architecture.Get<BattlePassService>();

        adObj.SetActive(!(subService.IsFreeRewardAd || bpService.IsPremium));
    }
    public override void Deactive()
    {
        base.Deactive();
        GameUIPanel.Instance.inputController.SetActive(true);

    }


    public override void Close()
    {
        if (isBusy) return;
        base.Close();
    }
    private async UniTask DOAnimation()
    {
        UniTask op = UniTask.Delay(0);
        for (int i = 0; i < _buffItems.Length; i++)
        {
            var buff = _buffItems[i];
            op = buff.PlayAnimation();
            await UniTask.Delay(100);
        }
        await op;
    }

    private async UniTask InitNewBuffs()
    {
        isBusy = true;
        boxObj.SetActive(false);
        foreach (var buffItem in _buffItems)
        {
            buffItem.gameObject.SetActive(true);
        }

        offerBtn.SetActive(false);
        closeBtn.SetActive(false);

        // get buffs
        var buffTable = DataManager.Base.Buff;
        var currentDataBuff = GameController.Instance.GetSession().buffSession.Dungeon.BuffEquiped;
        List<BuffEntity> entities = null;
        do
        {
            entities = buffTable.GetBuffs(_buffItems.Length, _typeBuff, DataManager.Save.User.HeroCurrent);
        }
        while (Contains(entities,EBuff.AngelWhisper));

        bool Contains(List<BuffEntity> current,EBuff target)
        {
            foreach(var buff in current)
            {
                if (buff.Type == target)
                {
                    return true;
                }
            }
            return false;
        }


        for (int i = 0; i < _buffItems.Length; i++)
        {
            _buffItems[i].SetEntity(entities[i]);
            _buffItems[i].PrepareAnimation();
        }

        // set buffs
        for (int i = 0; i < _buffItems.Length; i++)
        {
            _buffItems[i].SetInfor();
            var type = entities[i].Type;
            if (currentDataBuff.ContainsKey(type))
            {
                _buffItems[i].SetStarActive(currentDataBuff[type].Level);
            }
            else
            {
                _buffItems[i].SetStarActive(0);
            }
            _buffItems[i].SetDescription();
        }
        await DOAnimation();
        foreach(var buffItem in _buffItems)
        {
            buffItem.BuffOnClicked();
        }
        await UniTask.Delay(1000);
        Run().Forget();
        async UniTask Run()
        {
            for (int i = 0; i < _buffItems.Length; i++)
            {
                var buff = _buffItems[i];
                buff.DisappearAnimation();
                await UniTask.Delay(100);
            }

            isBusy = false;
            Close();
        }
    }
    public void GetOffer()
    {
        Architecture.Get<AdService>().ShowRewardedAd(AD.AdPlacementKey.BUFF_OFFER, (result) =>
        {
            if (result)
            {
                InitNewBuffs();
            }
            else
            {
                PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.NotAd"));
            }
        }, placement: AD.AdPlacementKey.BUFF_OFFER);
        //show ad
    }

#if DEVELOPMENT
    [Button]
    public void PostInitByEditor()
    {
        PostInit();
        Show();
    }
#endif
}
