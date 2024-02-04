using Assets.Game.Scripts.BaseFramework.Architecture;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.BattlePass.UI
{
    public class UIButtonBattlePass : UIBaseButton
    {
        [SerializeField] private TextMeshProUGUI seasonTxt;
        [SerializeField] private TextMeshProUGUI levelTxt;
        [SerializeField] private Image filledExpProgress;
        private BattlePassService service;
        protected override void OnEnable()
        {
            service = Architecture.Get<BattlePassService>();
            service.BattlePassAddExpEvent += Service_BattlePassAddExpEvent;
            base.OnEnable();
            Show();
        }

        private void Service_BattlePassAddExpEvent(int exp)
        {
            Show();
        }

        protected override void OnDisable()
        {
            service.BattlePassAddExpEvent -= Service_BattlePassAddExpEvent;
            base.OnDisable();
        }

        private void Show()
        {
            seasonTxt.text = I2Localize.GetLocalize("Common/Title_Season", service.Season + 1);
            levelTxt.text = service.Level.ToString();
            filledExpProgress.fillAmount = service.ProgressExp;
        }

        public override void Action()
        {
            var last = PanelManager.Instance.GetLast();
            last.HideByTransitions().Forget();

            PanelManager.CreateAsync(AddressableName.UIBattlePassPanel).ContinueWith(panel =>
            {
                panel.Show();
                panel.onClosed += last.ShowByTransitions;
            }).Forget();
        }
    }
}
