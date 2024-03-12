using BansheeGz.BGDatabase;
using ExtensionKit;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Buff
{
    [System.Serializable]
    public class BuffEntity
    {
        public int Id;
        public string Type;
        public ERarity Rarity;
        public string LocalizeKey;
        public List<ModifierData> ModifierPassive;
        public List<ModifierData> ModifierSkill;
        public string PrefabPath;
        public bool IsPrefabBuff => PrefabPath.IsNotNullAndEmpty();
        private BGEntity e;

        public BuffEntity()
        {
            ModifierPassive = new List<ModifierData>();
            ModifierSkill = new List<ModifierData>();
        }

        public BuffEntity(BGEntity e) : this()
        {
            this.e = e;
            Id = e.Get<int>("Id");
            Type = e.Get<string>("Type");
            Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
            LocalizeKey = e.Get<string>("LocalizeKey") ?? "";
            PrefabPath = e.Get<string>("PrefabPath") ?? "";

            var dataPassive = e.Get<List<string>>("Modifier_Passive");
            if (dataPassive.IsNotNull())
            {
                foreach (var data in dataPassive)
                {
                    var modi = new ModifierData(data);
                    ModifierPassive.Add(modi);
                }
            }

            var dataSkill = e.Get<List<string>>("Modifier_Skill");
            if (dataSkill.IsNotNull())
            {
                foreach (var data in dataSkill)
                {
                    var modi = new ModifierData(data);
                    ModifierSkill.Add(modi);
                }
            }
        }

        public BuffEntity Clone() => new BuffEntity(e);
    }
}
