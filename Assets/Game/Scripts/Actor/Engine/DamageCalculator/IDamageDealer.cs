using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface IDamageDealer
    {
        void Init(IStatGroup stat);
        void Release(IStatGroup stat);
        HitResult DealDamage(ActorBase attacker, ActorBase defender);
    }
}