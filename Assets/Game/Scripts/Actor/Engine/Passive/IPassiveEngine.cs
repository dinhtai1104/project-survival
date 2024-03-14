using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface IPassiveEngine
    {
        void Init(Actor owner);
        void OnUpdate();
        void AddPassive(IPassive passive);
        void RemovePassive(IPassive passive);
    }
}
