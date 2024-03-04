using Assets.Game.Scripts.Core.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class EnemyActor : Actor
    {
        public int MonsterLevel { set; get; }
        public EnemyEntity MonsterData { set; get; }
    }
}
