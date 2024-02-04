using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.GameActor
{
    public class EnemyNormal : EnemyBase
    {
        //private async UniTaskVoid Start()
        //{
        //    var test = GetTestStat();
        //    await SetUp(test);

        //    StartBehaviours();
        //}

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