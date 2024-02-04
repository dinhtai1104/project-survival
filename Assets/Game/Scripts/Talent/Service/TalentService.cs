using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Talent.Database;
using Assets.Game.Scripts.Talent.Datasave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Talent
{
    public class TalentService : Service, ITalentService
    {
        public delegate void TalentClaimDelegate(StatKey Type, int Id);


        private TalentsSave _save;
        private TalentTable _table;
        private ValueConfigSearch _costUnlockConfig = new ValueConfigSearch("[Talent]UpgradeCost", "Gold;1000");
        private ResourceData _costUnlockTalent;

        public ResourceData CostUnlockTalent { get => _costUnlockTalent; set => _costUnlockTalent = value; }

        public event TalentClaimDelegate OnClaimTalent;
        public override void OnInit()
        {
            base.OnInit();
            _save = DataManager.Save.Talent;
            _table = DataManager.Base.Talent;
            _costUnlockTalent = new ResourceData(_costUnlockConfig.StringValue);
        }

        public override void OnStart()
        {
            base.OnStart();
            ApplyTalent(GameSceneManager.Instance.PlayerData);
        }

        public void ClaimTalent(int Id)
        {
            _save.ClaimTalent(Id);
            var entity = _table.Get(Id);

            var attribute = entity.GetAttribute(GetLevel(Id));
            GameSceneManager.Instance.PlayerData.AddMondifierTalent(attribute);


            OnClaimTalent?.Invoke(entity.AttributeModifier.StatKey, Id);
        }

        public List<TalentEntity> GetRollTalent(int amount)
        {
            return _table.GetRandomTalent(amount);
        }

        public List<TalentEntity> GetRollTalentSameWeight(int amount)
        {
            return _table.GetRollTalentSameWeight(amount);
        }

        public bool HasUnlockTalent(int Id)
        {
            return _save.HasUnlockTalent(Id);
        }

        public bool IsMaxLevelTalent(int Id)
        {
            return _save.GetLevelTalent(Id) >= _table.GetTalentByIndex(Id).MaxLevel;
        }

        public void ApplyTalent(PlayerData playerData)
        {
            playerData.Stats.RemoveModifiersFromSource(EStatSource.Talent);
            foreach (var talentId in _save.Talents)
            {
                var entity = _table.GetTalentByIndex(talentId.Key);
                var attribute = entity.GetAttribute(talentId.Value);
                playerData.AddMondifierTalent(attribute);
            }
            Logger.Log("==========Apply Talent==========");
        }

        public void ApplyTalent(IStatGroup statGroup)
        {
            statGroup.RemoveModifiersFromSource(EStatSource.Talent);
            foreach (var talentId in _save.Talents)
            {
                var entity = _table.GetTalentByIndex(talentId.Key);
                var attribute = entity.GetAttribute(talentId.Value);
                statGroup.AddModifier(attribute.StatKey, attribute.Modifier, EStatSource.Talent);
            }
        }

        public float GetValueAttribute(StatKey statkey)
        {
            Stat stat = new Stat();
            foreach (var talentId in _save.Talents)
            {
                var entity = _table.GetTalentByIndex(talentId.Key);
                if (entity.StatKey != statkey) continue;
                var attribute = entity.GetAttribute(talentId.Value);
                stat.AddModifier(attribute.Modifier);
            }
            stat.RecalculateValue();
            return stat.Value;
        }

        public int GetLevel(int id)
        {
            return _save.GetLevelTalent(id);
        }

        public bool HasNotify()
        {
            var resource = DataManager.Save.Resources;
            if (resource.HasResource(_costUnlockTalent))
            {
                return true;
            }

            return false;
        }
    }
}
