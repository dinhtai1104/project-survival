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
        public Actor m_Actor;

        public HealthZeroEventArgs(Actor actor)
        {
            m_Actor = actor;
        }
    }
}
