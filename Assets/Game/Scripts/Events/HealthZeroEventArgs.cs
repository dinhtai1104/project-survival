using Core;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public class HealthZeroEventArgs : BaseEventArgs<HealthZeroEventArgs>
    {
        public ActorBase m_Actor;

        public HealthZeroEventArgs(ActorBase actor)
        {
            m_Actor = actor;
        }
    }
}
