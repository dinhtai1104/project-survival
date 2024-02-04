using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using UI;
using UnityEngine;
public class UIBuffChoicePanel : UI.Panel
{
    private ValueConfigSearch costReroll = new ValueConfigSearch("[Buff_Panel]Cost_Reroll", "Resource;Gem;50");
    [SerializeField] private EPickBuffType _typeBuff;
    [SerializeField] private UIBuffItem[] _buffItems;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject buttonReroll;
    [SerializeField] private UIIconText rerollPanel;
    public override void PostInit()
    {
        Messenger.AddListener<EPickBuffType>(EventKey.PickBuffDone, OnPickBuffDone);
        buttonReroll.SetActive(false);
    }

    public override void Show()
    {
        base.Show();
        rerollPanel.Set(new LootParams(costReroll.StringValue).Data);
        InitNewBuffs();
    }

    private void OnPickBuffDone(EPickBuffType type)
    {
        Run().Forget();
        async UniTask Run()
        {
            canvasGroup.blocksRaycasts = false;
            for (int i = 0; i < _buffItems.Length; i++)
            {
                var buff = _buffItems[i];
                buff.DisappearAnimation();
                await UniTask.Delay(100);
            }
            Close();
        }
    }

    public override void Close()
    {
        base.Close();
        Messenger.RemoveListener<EPickBuffType>(EventKey.PickBuffDone, OnPickBuffDone);
    }
    private async UniTask DOAnimation()
    {
        UniTask op = UniTask.Delay(0);
        for (int i = 0; i < _buffItems.Length; i++)
        {
            var buff = _buffItems[i];
            op = buff.PlayAnimation(0.4f + 0.5f * i);
            await UniTask.Delay(100);
        }
        await op;
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
        if (DataManager.Save.User.SessionGame > 0)
        {
            buttonReroll.SetActive(true);
        }
    }

    private void InitNewBuffs()
    {
        // get buffs
        var buffTable = DataManager.Base.Buff;
        var currentDataBuff = GameController.Instance.GetSession().buffSession.Dungeon.BuffEquiped;
        var entities = buffTable.GetBuffs(_buffItems.Length, _typeBuff, DataManager.Save.User.HeroCurrent);
        entities.MMShuffle();
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
        DOAnimation();
    }
    public void OnRerollBuff()
    {
        
        var cost = new LootParams(costReroll.StringValue).Data as ResourceData;
        if (DataManager.Save.Resources.HasResource(cost))
        {
            DataManager.Save.Resources.DecreaseResource(cost);
            InitNewBuffs();
            GameController.Instance.RerollBuff("gem");
        }
        else
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, cost.Resource.GetLocalize())).Forget();
        }
    }

    [SerializeField]
    private WeaponBase[] tests;
    public void SelectSupportWeapon()
    {
        var player = Game.Controller.Instance.gameController.GetMainActor();

        player.WeaponHandler.SetSupportWeapon(tests[UnityEngine.Random.Range(0, tests.Length)]).Forget();
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
