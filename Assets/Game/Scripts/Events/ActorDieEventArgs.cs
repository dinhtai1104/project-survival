using Core;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public class ActorDieEventArgs : BaseEventArgs<ActorDieEventArgs>
    {
        public ActorBase m_Actor;

        public ActorDieEventArgs(ActorBase actor)
        {
            m_Actor = actor;
        }
    }
}
