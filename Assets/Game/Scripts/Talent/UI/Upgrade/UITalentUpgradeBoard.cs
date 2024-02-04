using Assets.Game.Scripts.Talent.Database;
using Assets.Game.Scripts.Talent.Enums;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;
using UnityEngine;

namespace Assets.Game.Scripts.Talent.UI.Upgrade
{
    public class UITalentUpgradeBoard : MonoBehaviour
    {
        [SerializeField] private List<UITalentUpgradeViewItem> talentItems;
        [SerializeField] private CanvasGroup boardCanvas;
        private TalentService _service;
        private UITalentUpgradePanel panel;
        private UITalentPanel _parentPanel;

        [SerializeField] private GameObject buttons;

        public void Show(TalentService _service, UITalentUpgradePanel panel, UITalentPanel _parentPanel)
        {
            this.panel = panel;
            this._parentPanel = _parentPanel;
            this._service = _service;
            var talents = GetTalent(ETalentRoll.First);
            Play(talents);
        }

        private async void Play(List<TalentEntity> talents)
        {
            buttons.SetActive(false);
            boardCanvas.interactable = false;

            var list = new List<UniTask>();

            for (int i = 0; i < talents.Count; i++)
            {
                talentItems[i].gameObject.SetActive(true);
                var task = talentItems[i].Run(this, _service, talents[i], 1f + i * 0.5f, 16 + i * 2);
                list.Add(task);
            }
            for (int i = talents.Count; i < talentItems.Count; i++)
            {
                talentItems[i].gameObject.SetActive(false);
            }
            await UniTask.WhenAll(list);

            buttons.SetActive(true);
            boardCanvas.interactable = true;
        }

        public void DisactiveAllButton()
        {
            boardCanvas.interactable = false;
        }

        public void PickThis(TalentEntity target)
        {
            _parentPanel.ScrollToTalent(target.Id);
            panel.onClosed += () =>
            {
                _service.ClaimTalent(target.Id);
            };
            panel.Close();
        }

        private List<TalentEntity> GetTalent(ETalentRoll rollType)
        {
            if (rollType == ETalentRoll.First)
            {
                return _service.GetRollTalent(3);
            }
            else
            {
                return _service.GetRollTalentSameWeight(3);
            }
        }

        public void RerollOnClicked()
        {
            Play(GetTalent(ETalentRoll.Extra));
        }
    }
}
