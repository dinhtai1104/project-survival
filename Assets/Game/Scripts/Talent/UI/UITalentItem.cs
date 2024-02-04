using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Talent.Database;
using Assets.Game.Scripts.Utilities;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Talent.UI
{
    public class UITalentItem : UIBaseButton
    {
        private TalentEntity entity;
        [SerializeField] private Image qualityImg;
        [SerializeField] private Image skillImg;
        [SerializeField] private TextMeshProUGUI modifierText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI nameText;

        [SerializeField] private GameObject lockObject;
        [SerializeField] private GameObject unlockObject;

        private TalentService _service;

        public TalentEntity Entity { get => entity; set => entity = value; }

        public void Setup(TalentEntity entity)
        {
            this.Entity = entity;
            _service = Architecture.Get<TalentService>();
            SetInformationItemTalent();
        }

        private void SetInformationItemTalent()
        {
            qualityImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentRarity, Entity.Quality.ToString());

            bool isUnlock = _service.HasUnlockTalent(Entity.Id);
            if (!isUnlock)
            {
                unlockObject.SetActive(false);
                lockObject.SetActive(true);
                button.interactable = false;
                return;
            }
            button.interactable = true;
            unlockObject.SetActive(true);
            lockObject.SetActive(false);

            levelText.text = _service.GetLevel(Entity.Id).ToString();
            skillImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Talent, Entity.StatKey.ToString());
            var attribute = Entity.GetAttribute(_service.GetLevel(Entity.Id));
            modifierText.text = attribute.Modifier.ToString(entity.Format);
            nameText.text = I2Localize.GetLocalize("Stat/" + attribute.StatKey);
        }

        private void UITalentItem_OnClaimTalent(StatKey Type, int Id)
        {
            if (Entity.StatKey != Type) return;
            if (Entity.Id != Id) return;
            SetInformationItemTalent();
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            Architecture.Get<TalentService>().OnClaimTalent += UITalentItem_OnClaimTalent;
        }

        protected override void OnDisable()
        {
            DOTween.Kill(gameObject);
            base.OnDisable();
            Architecture.Get<TalentService>().OnClaimTalent -= UITalentItem_OnClaimTalent;
        }

        public override void Action()
        {
        }

        public void PunchScale()
        {
            Sequence sequence = DOTween.Sequence(gameObject).SetUpdate(true);

            sequence.Append(button.transform.DOScale(Vector3.one * 1.3f * originScale, 0.2f))
                    .Append(button.transform.DOScale(Vector3.one * 1f * originScale, 0.1f));
        }
    }
}
