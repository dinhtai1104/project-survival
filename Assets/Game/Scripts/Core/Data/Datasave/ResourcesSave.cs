using Assets.Game.Scripts.DataGame.Loot;
using Assets.Game.Scripts.Enums;
using Assets.Game.Scripts.Gameplay.DataLocal;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Datasave
{
    [System.Serializable]
    public class ResourcesSave : BaseDatasave
    {
        [ShowInInspector]
        public Dictionary<EResource, double> Resources;
        public DateTime LastTimeChargeEnergy;


        public ResourcesSave()
        {
        }

        public ResourcesSave(string key) : base(key)
        {
            Resources = new Dictionary<EResource, double>();
            LastTimeChargeEnergy = UnbiasedTime.UtcNow;
        }

        public void IncreaseResources(params ResourceData[] data)
        {
            foreach (var dt in data)
            {
                IncreaseResource(dt.Resource, dt.Value, false);
            }
            Save();
        }


        [Button]
        public void IncreaseResource(EResource Resource, double value, bool save = true)
        {
            if (!Resources.ContainsKey(Resource))
            {
                Resources.Add(Resource, 0);
            }
            Resources[Resource] += value;
        }
        public void IncreaseResource(ResourceData data, bool save = true)
        {
            IncreaseResource(data.Resource, data.Value, save);
        }
        public void DecreaseResource(EResource Resource, double value, bool save = true)
        {
            if (!Resources.ContainsKey(Resource)) return;
            Resources[Resource] -= value;
            if (Resources[Resource] <= 0)
            {
                Resources[Resource] = 0;
            }
            if (save)
            {
                Save();
            }
        }

        public void DecreaseResources(params ResourceData[] data)
        {
            foreach (var dt in data)
            {
                DecreaseResource(dt.Resource, dt.Value, false);
            }
            Save();
        }
        public void DecreaseResources(IEnumerable<ResourceData> data)
        {
            foreach (var dt in data)
            {
                DecreaseResource(dt.Resource, dt.Value, false);
            }
            Save();
        }
        public void DecreaseResource(ResourceData data, bool save = true)
        {
            DecreaseResource(data.Resource, data.Value, save);
        }
        public override void Fix()
        {
            
        }
      
        public double GetResource(EResource type)
        {
            if (Resources.ContainsKey(type) == false)
            {
                return 0;
            }
            return Resources[type];
        }
        public bool HasResource(IEnumerable<ResourceData> cost)
        {
            foreach (var m in cost)
            {
                if (!HasResource(m)) return false;
            }
            return true;
        }
        public bool HasResource(ResourceData cost)
        {
            return GetResource(cost.Resource) >= cost.Value;
        }

        public void IncreaseResource(List<LootParams> lootData)
        {
            foreach (var item in lootData)
            {
                var rs = item.Data as ResourceData;
                IncreaseResource(rs);
            }
        }

        public bool HasResource(EResource resource, int target)
        {
            return GetResource(resource) >= target;
        }
    }

}
