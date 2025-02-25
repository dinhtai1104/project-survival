using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorState
{
    public class BornState : BaseCharacterState
    {
        public override void Enter()
        {
            base.Enter();
            Actor.IsActivated = true;
            ToIdleState();
        }
    }
}
