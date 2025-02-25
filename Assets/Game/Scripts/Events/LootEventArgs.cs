using Assets.Game.Scripts.Enums;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Events
{
    public class LootEventArgs : BaseEventArgs<LootEventArgs>
    {
        public ELootObject Type;

        public LootEventArgs(ELootObject type)
        {
            Type = type;
        }
    }
}
