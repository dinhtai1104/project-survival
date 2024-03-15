using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface IPassive
    {
        ActorBase Owner { get; set; }
        void OnUpdate();
        void UnEquip();
        void Equip();
    }
}
