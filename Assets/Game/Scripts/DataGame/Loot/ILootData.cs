using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.DataGame.Loot
{
    public interface ILootData
    {
        double ValueLoot { get; }
        bool CanMergeData { get; }
        bool Add(ILootData data);
        void Loot();
        List<LootParams> GetAllData();
        ILootData CloneData();
        void Multiply(float multi);
        string GetParams();
    }
}
