using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Subscription.Services;
using Sirenix.OdinInspector;
using UnityEngine;
public class UISubscriptionPanel : UI.Panel
{
    [SerializeField] private UISubscriptionItem m_Normal;
    [SerializeField] private UISubscriptionItem m_Vip;
    public override void PostInit()
    {
    }

    [Button]
    public override void Show()
    {
        base.Show();
        var db = DataManager.Base.Subscription;
        var service = Architecture.Get<SubscriptionService>();
        m_Normal.Init(db.Get(0), service.save.Saves[ESubscription.Normal]);
        m_Vip.Init(db.Get(1), service.save.Saves[ESubscription.Vip]);
    }
}
