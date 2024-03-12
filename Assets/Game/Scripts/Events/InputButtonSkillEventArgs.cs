using Core;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Events
{
    public class InputButtonSkillEventArgs : BaseEventArgs<InputButtonSkillEventArgs>
    {
        public EControlCode ControlCode;
    }
}
