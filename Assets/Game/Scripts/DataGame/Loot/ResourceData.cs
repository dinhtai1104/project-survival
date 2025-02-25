using Assets.Game.Scripts.DataGame.Loot;
using Assets.Game.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Gameplay.DataLocal
{
    [System.Serializable]
    public class ResourceData : ILootData
    {
        public double ValueLoot => Value;
        public EResource Resource;
        public double Value;
        private List<string> rawData;

        public bool CanMergeData => true;
        public ResourceData() { }
        public ResourceData(List<string> param)
        {
            Enum.TryParse(param[0], out Resource);
            Value = double.Parse(param[1]);
            if (param.Count > 2)
            {
                var rd = long.Parse(param[2]);
                Value = (long)UnityEngine.Random.Range((int)Value, rd + 1);
            }
        }
        public ResourceData(string content)
        {
            var split = content.Split(';');
            Enum.TryParse(split[0], out Resource);
            Value = long.Parse(split[1]);

            if (split.Length > 2)
            {
                var rd = long.Parse(split[2]);
                Value = (long)UnityEngine.Random.Range((int)Value, rd + 1);
            }
        }

        public ResourceData(EResource type, double value)
        {
            Resource = type;
            Value = value;
        }

        public void Multiply(float value)
        {
            Value = (long)Mathf.Ceil((float)Value * value);
        }
        public bool Add(ILootData data)
        {
            var resource = data as ResourceData;
            if (resource != null)
            {
                if (resource.Resource == Resource)
                {
                    Value += resource.Value;
                    if (Value <= 0) Value = 0;

                    return true;
                }
            }
            return false;
        }
        public ResourceData Clone()
        {
            return new ResourceData { Resource = Resource, Value = Value };
        }
        public override string ToString()
        {
            return $"{Resource} {Value}";
        }
        public void Loot()
        {
            DataManager.Save.Resources.IncreaseResource(Clone());
        }

        public List<LootParams> GetAllData()
        {
            var ilist = new List<LootParams>();
            return ilist;
        }
        public string GetParams()
        {
            return $"Resource;{Resource};{Value}";
        }
        public ILootData CloneData()
        {
            return Clone();
        }
    }
}
