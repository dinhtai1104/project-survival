using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.State.Common
{
    public class ActorSkillState : BaseSkillState
    {
        public override void Enter()
        {
            base.Enter();
            Actor.Health.Invincible = true;
        }
        public override void Exit()
        {
            base.Exit();
            Actor.Health.Invincible = false;
        }
    }
}
