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
            GameArchitecture.GetService<IEventMgrService>().Fire(this, new LootEventArgs(Type));
        }
    }
}
