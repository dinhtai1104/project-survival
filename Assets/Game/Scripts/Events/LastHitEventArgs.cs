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
        public Actor m_Killer;
        public Actor m_Patient;

        public LastHitEventArgs(Actor killer, Actor patient)
        {
            m_Killer = killer;
            m_Patient = patient;
        }
    }
}
