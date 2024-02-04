using Assets.Game.Scripts.BaseFramework.Architecture;
using UnityEngine;
using UI;

namespace Assets.Game.Scripts.Talent.UI.Upgrade
{
    public class UITalentUpgradePanel : Panel
    {
        private TalentService _service;
        [SerializeField] private UITalentUpgradeBoard _boardPickTalent;
        private UITalentPanel _parentPanel;
        public override void PostInit()
        {
        }

        public void Show(UITalentPanel _parentTalent)
        {
            this._parentPanel = _parentTalent;
            _service = Architecture.Get<TalentService>();
            base.Show();
            _boardPickTalent.Show(_service, this, _parentPanel);
        }
    }
}
