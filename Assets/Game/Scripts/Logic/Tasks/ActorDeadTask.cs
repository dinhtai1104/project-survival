using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class ActorDeadTask : SkillTask
    {
        public override async UniTask Begin()
        {
            await base.Begin();
            Caster.DeadForce();
            Skill.Stop();
        }
    }
}
