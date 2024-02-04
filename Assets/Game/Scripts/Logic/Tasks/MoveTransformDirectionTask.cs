using Cysharp.Threading.Tasks;
using Game.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class MoveTransformDirectionTask : SkillTask
    {
        public Transform transformMove;
        public Vector3 direction;
        public override async UniTask Begin()
        {
            await base.Begin();
            transformMove.position += direction;
            IsCompleted = true;
        }
    }
}
