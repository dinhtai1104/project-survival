using Core;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Events
{
    public class LastHitEventArgs : BaseEventArgs<LastHitEventArgs>
    {
        public ActorBase m_Killer;
        public ActorBase m_Patient;

        public LastHitEventArgs(ActorBase killer, ActorBase patient)
        {
            m_Killer = killer;
            m_Patient = patient;
        }
    }
}
