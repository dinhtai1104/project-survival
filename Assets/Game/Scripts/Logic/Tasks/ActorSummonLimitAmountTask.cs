using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class ActorSummonLimitAmountTask : ActorSummonTaskLimit
    {
        public ValueConfigSearch numberEnemy;
        public ValueConfigSearch spaceFromCenter;
        public ValueConfigSearch distanceBtwEnemy;
        protected List<Vector3> preparePosition = new List<Vector3>();

        public override UniTask Begin()
        {
            PreparePosition();
            return base.Begin();
        }

        protected virtual void PreparePosition()
        {
            preparePosition = GameUtility.GameUtility.GetRandomPositionWithoutOverlapping
              (Caster.GetMidPos(), spaceFromCenter.Vector2Value, distanceBtwEnemy.FloatValue, numberEnemy.IntValue, ignoreMask, true);
        }
        protected override void SpawnMini()
        {
            var left = maxMiniEnemies.IntValue - miniEnemies.Count;
            var leftCanSpawn = Mathf.Min(left, numberEnemy.IntValue);
            for (int i = 0; i < leftCanSpawn; i++)
            {
                Spawn(preparePosition[i]);
            }
            IsCompleted = true;
        }
    }
}
