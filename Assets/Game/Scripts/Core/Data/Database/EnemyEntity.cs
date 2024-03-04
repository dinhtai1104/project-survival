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
        public int Id;
        public string Name;
        public string Path;

        public float Hp;
        public float Damage;
        public float Speed;
        public float AttackSpeed;

        public float iHp;
        public float iDamage;
        public float iSpeed;
        public float iAttackSpeed;

     
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
    }
}
