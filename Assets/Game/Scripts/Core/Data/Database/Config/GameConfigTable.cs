using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Config
{
    [System.Serializable]
    public class GameConfigTable : DataTable<string, string>
    {
        public override void GetDatabase()
        {
            DB_GameConfig.ForEachEntity(e => Get(e));
            DB_EnemyConfig.ForEachEntity(e => Get(e));
        }

        private void Get(BGEntity e)
        {
            var configKey = e.Get<string>("ConfigKey");
            var configValue = e.Get<string>("ConfigValue");

            if (string.IsNullOrEmpty(configValue)) { return; }
            if (string.IsNullOrEmpty(configKey)) { return; }

            if (Dictionary.ContainsKey(configKey)) { return; }
            Dictionary.Add(configKey, configValue);
        }

        public string GetValue(string key, string defaultValue)
        {
            if (Dictionary.ContainsKey(key) == false) return defaultValue;
            return Get(key);
        }
    }
}
