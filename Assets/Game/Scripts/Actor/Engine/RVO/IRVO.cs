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
        void Init(Actor  owner);
        void OnUpdate();
        Vector2 NextPosition();
    }
}
