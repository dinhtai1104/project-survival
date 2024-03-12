using BansheeGz.BGDatabase;
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
    }
}
