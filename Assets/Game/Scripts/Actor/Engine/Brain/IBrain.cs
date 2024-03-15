using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface IBrain
    {
        ActorBase Owner { get; }
        bool Lock { set; get; }
        void Init(ActorBase actor);
        void OnUpdate();
    }
}
