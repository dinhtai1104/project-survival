using Assets.Game.Scripts.Events;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Objects.Loots
{
    public class ExpObject : LootableObject
    {
        public override void Loot()
        {
            base.Loot();
            Architecture.Get<EventMgr>().Fire(this, new LootEventArgs(Type));
        }
    }
}
