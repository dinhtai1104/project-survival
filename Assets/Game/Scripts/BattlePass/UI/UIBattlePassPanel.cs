using Assets.Game.Scripts.BaseFramework.Architecture;
using Coffee.UIEffects;
using com.mec;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
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
    public class UIBattlePassPanel : Panel
    {
        [SerializeField] private UIBattlePassContainer _container;
        private BattlePassService service;
        [SerializeField] private UIShiny effectPremium;
        [SerializeField] private ParticleSystem m_ParticleEffectPremium;
        [SerializeField] private TextMeshProUGUI levelTxt;
        [SerializeField] private TextMeshProUGUI seasonTxt;
        [SerializeField] private TextMeshProUGUI timeEndTxt;
        [SerializeField] private Image filledLevel;

        public override void PostInit()
        {
        }
        public override void Show()
        {
            Messenger.AddListener(EventKey.BuyPremiumBattlePass, OnBuyPremiumBattlePass);
            service = Architecture.Get<BattlePassService>();
            service.BattlePassAddExpEvent += Service_BattlePassAddExpEvent;
            base.Show();
            UpdateProgress(service.Level);
            _container.Show();

            effectPremium.enabled = service.IsPremium;
            if (service.IsPremium)
            {
                m_ParticleEffectPremium.Play();
            }
            else
            {
                m_ParticleEffectPremium.Stop();
            }
            seasonTxt.text = I2Localize.GetLocalize("Common/Title_Season", service.Season + 1);
            if (service.IsRunning)
            {
                Timing.RunCoroutine(_Ticks(), Segment.RealtimeUpdate, gameObject);
            }
            else
            {
                Timing.KillCoroutines(gameObject);
                timeEndTxt.text = I2Localize.GetLocalize("Common/Title_BattlePass_NotRunning");
            }
        }

        private IEnumerator<float> _Ticks()
        {
            var end = service.TimeEnd;
            while(true)
            {
                var left = end - UnbiasedTime.UtcNow;
                if (left.TotalSeconds <= 0) break;
                timeEndTxt.text = left.ConvertTimeToString().ToLower();
                yield return Timing.WaitForSeconds(1);
            }
            service.Reset();
            Close();
        }

        private void OnBuyPremiumBattlePass()
        {
            effectPremium.enabled = true;
            m_ParticleEffectPremium.Play();
        }

        private void Service_BattlePassAddExpEvent(int exp)
        {
            UpdateProgress(service.Level);
        }

        public override void Close()
        {
            Timing.KillCoroutines(gameObject);
            Messenger.RemoveListener(EventKey.BuyPremiumBattlePass, OnBuyPremiumBattlePass);
            service.BattlePassAddExpEvent -= Service_BattlePassAddExpEvent;
            base.Close();
        }

        [Button]
        public void UpdateProgress(int level)
        {
            level -= 1;
            if (level < 1) level = 0;

            _container.SetProgress(level);
            levelTxt.text = (service.Level).ToString();
            filledLevel.fillAmount = service.ProgressExp;
        }
    }
}
