using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database
{
    [System.Serializable]
    public class EnemyEntity
    {
        public string Id;
        public string Path;

        public float Hp;
        public float Damage;
        public float Speed;
        public float AttackSpeed;
        public float AttackRange;

        public float iHp;
        public float iDamage;
        public float iSpeed;
        public float iAttackSpeed;

        public float Mass;

     
        // Monster Tags
        public List<string> Tags;

        public bool HasTag(string tag)
        {
            foreach (var t in Tags)
            {
                if (string.Compare(t, tag, StringComparison.Ordinal) == 0) return true;
            }

            return false;
        }

        public EnemyEntity(BGEntity e)
        {
            Id = e.Get<string>("Id");
            Path = e.Get<string>("Path");

            Hp = e.Get<float>("Hp");
            Damage = e.Get<float>("Damage");
            Speed = e.Get<float>("Speed");
            AttackSpeed = e.Get<float>("AttackSpeed");
            AttackRange = e.Get<float>("AttackRange");

            iHp = e.Get<float>("iHp");
            iDamage = e.Get<float>("iDamage");
            iSpeed = e.Get<float>("iSpeed");
            iAttackSpeed = e.Get<float>("iAttackSpeed");

            Tags = e.Get<List<string>>("Tags");

            //Mass = e.Get<float>("Mass");
        }
    }
}
