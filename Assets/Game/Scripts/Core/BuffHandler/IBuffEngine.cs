using Assets.Game.Scripts.DataGame.Data;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.BuffHandler
{
    public interface IBuffEngine
    {
        Actor Owner { get; }
        void Init(Actor owner);
        void AddBuff(BuffData buff);
        void Debuff(BuffData buff);
        BuffData GetBuff(int Id);
        BuffData GetBuff(string Type);
        void OnUpdate();
    }
}
