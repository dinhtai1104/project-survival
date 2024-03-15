using BansheeGz.BGDatabase;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Buff
{
    [System.Serializable]
    public class BuffTable : DataTable<int, BuffEntity>
    {
        private Dictionary<string, BuffEntity> m_LookupBuffByType;
        public override void GetDatabase()
        {
            m_LookupBuffByType = new Dictionary<string, BuffEntity>();
            DB_Buff.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var buff = new BuffEntity(e);
            Dictionary.Add(buff.Id, buff);
            m_LookupBuffByType.Add(buff.Type, buff);
        }

        public BuffEntity GetBuffByType(string type)
        {
            return m_LookupBuffByType[type];
        }

        public List<BuffEntity> FilterByRarity(ERarity rarity)
        {
            var list = new List<BuffEntity>();

            foreach (var entity in Dictionary.Values)
            {
                if (entity.Rarity == rarity)
                {
                    list.Add(entity);
                }
            }
            return list;
        }
    }
}
