using Game.Fsm;
using Game.GameActor;
using UnityEngine;

namespace Game.AI.State
{
    public class ActorDeadStateSpawnAlly : BaseState
    {
        [SerializeField] private int numberChild = 3;
        [SerializeField] private string childId = "Enemy0";

        public override async void Enter()
        {
            base.Enter();
            Messenger.Broadcast(EventKey.EnemyAllySpawn);
            int dir = 1;
            for (int i = 0; i < numberChild; i++)
            {
                var enemy = await Game.Controller.Instance.gameController.GetEnemySpawnHandler().SpawnSingle(
                    childId,
                    (int)Actor.Stats.GetValue(StatKey.Level),
                    Actor.GetPosition());
                enemy.MoveHandler.Move(dir * Vector2.right, 1);

                dir *= -1;
            }
        }
    }
}