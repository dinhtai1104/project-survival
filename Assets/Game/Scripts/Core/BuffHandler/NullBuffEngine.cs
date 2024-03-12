using Assets.Game.Scripts.DataGame.Data;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.BuffHandler
{
    public class NullBuffEngine : IBuffEngine
    {
        public Actor Owner => null;

        public void AddBuff(BuffData buff)
        {
        }

        public void Debuff(BuffData buff)
        {
        }

        public BuffData GetBuff(int Id)
        {
            return new BuffData();
        }

        public BuffData GetBuff(string Type)
        {
            return new BuffData();
        }

        public void Init(Actor owner)
        {
        }

        public void OnUpdate()
        {
            
        }
    }
}
