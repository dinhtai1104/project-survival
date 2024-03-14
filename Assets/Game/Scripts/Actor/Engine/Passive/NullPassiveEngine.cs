using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class NullPassiveEngine : IPassiveEngine
    {
        public void AddPassive(IPassive passive)
        {
        }

        public void Init(Actor owner)
        {
        }

        public void OnUpdate()
        {
        }

        public void RemovePassive(IPassive passive)
        {
        }
    }
}
