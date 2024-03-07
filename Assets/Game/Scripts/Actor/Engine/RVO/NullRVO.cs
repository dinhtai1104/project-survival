using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine
{
    public class NullRVO : IRVO
    {
        public int Id => 0;
        public void Init(Actor owner)
        {
        }

        public Vector2 NextPosition()
        {
            return Vector2.zero;
        }

        public void OnUpdate()
        {
        }
    }
}
