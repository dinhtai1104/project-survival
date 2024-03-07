using Assets.Game.Scripts.Enums;
using Assets.Game.Scripts.Gameplay.DataLocal;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.DataGame.Loot
{
    [System.Serializable]
    public class LootParams
    {
        public ELootType Type;
        public List<string> Params;
        public string DataRaw;

        [ShowInInspector]
        public ILootData data;

        public virtual ILootData Data { get => data; set => data = value; }

        public LootParams() { }

        public LootParams(ELootType type, ILootData data)
        {
            this.Type = type;
            this.Data = data;
        }

        public LootParams(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            DataRaw = data;
            Params = new List<string>();
            var split = data.Split(';');
            System.Enum.TryParse(split[0], out Type);
            for (int i = 1; i < split.Length; i++)
            {
                Params.Add(split[i]);
            }

            InitData();
        }

        private void InitData()
        {
            switch (Type)
            {
                case ELootType.Resource:
                    Data = new ResourceData(Params);
                    break;
            }
        }

        public override string ToString()
        {
            return Data.ToString();
        }
        public LootParams Clone()
        {
            return new LootParams { Type = Type, Data = Data.CloneData() };
        }

        public void Multiply(float v)
        {
            Data.Multiply(v);
        }
    }
}
