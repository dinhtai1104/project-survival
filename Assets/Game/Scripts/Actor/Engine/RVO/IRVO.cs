using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine
{
    public interface IRVO
    {
        int Id { get; }
        void Init(ActorBase owner);
        void ReInit();
        void OnUpdate();
        Vector2 NextPosition();
    }
}
