using Core;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public class HealthFullEventEventArgs : BaseEventArgs<HealthFullEventEventArgs>
    {
        public ActorBase m_Actor;

        public HealthFullEventEventArgs(ActorBase actor)
        {
            m_Actor = actor;
        }
    }
}
