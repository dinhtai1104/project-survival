using System;

namespace Game.Tasks
{
    public class MoveToTargetTask : SkillTask
    {
        public float _thresholdDistance = 2;
        public bool isUseAnimStop = true;
        public override void Run()
        {
            base.Run();
            if (!Caster.MoveHandler.isGrounded) return;
            var target = Caster.FindClosetTarget();
            if (target == null)
            {
                return;
            }
            var direction = (target.GetPosition() - Caster.GetPosition()).normalized;
            Caster.MoveHandler.Move(direction, 1);

            if (Math.Abs(Caster.GetPosition().x - target.GetPosition().x) < _thresholdDistance) 
            {
                IsCompleted = true;
                Caster.MoveHandler.Stop(isUseAnimStop);
            }
        }
    }
}