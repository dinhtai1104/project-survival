using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.GameActor
{
    public class EnemyFlyNormal : EnemyBase
    {
        protected override async UniTask SetUp()
        {
            GetRigidbody().gravityScale = 0;
            await base.SetUp();
        }
        private IStatGroup GetTestStat()
        {
            IStatGroup stats = new StatGroup();
            stats.AddStat(StatKey.Dmg, 10);
            stats.AddStat(StatKey.Hp, 500);
            stats.AddStat(StatKey.SpeedMove, 2);

            stats.CalculateStats();
            return stats;
        }
    }
}