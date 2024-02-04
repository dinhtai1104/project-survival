using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Talent.Database
{
    [System.Serializable]
    public class TalentEntity : IWeightable
    {
        public int Id;
        public ERarity Quality;
        public AttributeStatModifier AttributeModifier;
        public int MaxLevel;
        public float Point;
        public float Weight => Point;
        public StatKey StatKey => AttributeModifier.StatKey;
        public string Format;

        private BGEntity talentEntity;
        public TalentEntity(BGEntity e)
        {
            this.talentEntity = e;
            Id = e.Get<int>("Id");
            Enum.TryParse(e.Get<string>("Quality"), out Quality);

            Enum.TryParse(e.Get<string>("StatName"), out StatKey statName);
            Enum.TryParse(e.Get<string>("ModifyType"), out EStatMod statMod);
            var modVal = e.Get<float>("ModifyValue");

            MaxLevel = e.Get<int>("MaxLevel");
            Point = e.Get<float>("Weight");
            Format = e.Get<string>("Format");
            AttributeModifier = new AttributeStatModifier(statName, new StatModifier(statMod, modVal));
        }

        public TalentEntity Clone()
        {
            return new TalentEntity(talentEntity);
        }

        public AttributeStatModifier GetAttribute(int level)
        {
            var clone = AttributeModifier.Clone();
            clone.Modifier.Value *= level;

            return clone;
        }
    }
}
