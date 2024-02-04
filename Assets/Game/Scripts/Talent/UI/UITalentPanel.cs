using UnityEngine;
using UI;
using Cysharp.Threading.Tasks;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Talent.UI.Upgrade;

namespace Assets.Game.Scripts.Talent.UI
{
    public class UITalentPanel : Panel
    {
        [SerializeField] private UITalentContainer _container;
        private ResourcesSave resource;
        [SerializeField] private UIIconText costUpgradeUI;
        private ResourceData cost;
        private TalentService _service;
        public override void PostInit()
        {
            _service = Architecture.Get<TalentService>();
            resource = DataManager.Save.Resources;
            cost = _service.CostUnlockTalent;
            costUpgradeUI.Set(cost);
        }
        public override void Show()
        {
            base.Show();
            _container.Show();
            _service.OnClaimTalent += _service_OnClaimTalent;
        }
        public override void Close()
        {
            base.Close();
            _service.OnClaimTalent -= _service_OnClaimTalent;
        }
        private async void _service_OnClaimTalent(StatKey Type, int Id)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));
            _container.EffectUpgrade(Id);
        }

        public void ScrollToTalent(int talentId)
        {
            _container.ScrollToIndex(talentId);
        }

        public void UpradeOnClicked()
        {
            if (resource.HasResource(cost))
            {
                resource.DecreaseResources(cost);

                PanelManager.CreateAsync<UITalentUpgradePanel>(AddressableName.UITalentUpgradePanel).ContinueWith(ui =>
                {
                    ui.Show(this);
                }).Forget();
            }
            else
            {
                PanelManager.ShowNotice(I2Localize.I2_NoticeNotEnough.AddParams(EResource.Gold)).Forget();
            }
        }
    }
}
