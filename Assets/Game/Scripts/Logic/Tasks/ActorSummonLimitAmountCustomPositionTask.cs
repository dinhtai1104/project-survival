using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class ActorSummonLimitAmountCustomPositionTask : ActorSummonLimitAmountTask
    {
        public ValueConfigSearch positionCenterSpawn;
        protected override void PreparePosition()
        {
            positionCenterSpawn.SetId(Caster.gameObject.name);
            preparePosition = GameUtility.GameUtility.GetRandomPositionWithoutOverlapping
                          (positionCenterSpawn.Vector2Value, spaceFromCenter.Vector2Value, distanceBtwEnemy.FloatValue, numberEnemy.IntValue);
        }
    }
}
