using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Enemies.CastOnState
{
    public interface IActionEnterState
    {
        void OnEnterState(ActorBase Actor);
    }
    public interface IActionExitState
    {
        void OnExitState(ActorBase Actor);
    }
}
