using UnityEngine;

namespace Assets.Game.Scripts.Tasks.Enemy
{
    public class WaitTask : Engine.Task
    {
        [SerializeField] private BindConfig m_WaitTime;
        private float timeCooldown = 0;
        public override void Begin()
        {
            base.Begin();
            timeCooldown = 0;
        }
        public override void Run()
        {
            base.Run();
            timeCooldown += Time.deltaTime;
            if (timeCooldown > m_WaitTime.FloatValue)
            {
                IsCompleted = true;
                return;
            }
        }
    }
}
