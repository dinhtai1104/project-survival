using Assets.Game.Scripts.BaseFramework.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Talent.Datasave
{
    public class TalentsSave : BaseDatasave
    {
        public Dictionary<int, int> Talents;
        public TalentsSave(string key) : base(key)
        {
            Talents = new Dictionary<int, int>();
        }

        public void ClaimTalent(int Id)
        {
            if (!Talents.ContainsKey(Id))
            {
                Talents.Add(Id, 0);
            }
            Talents[Id]++;
            Save();
        }
        public bool HasUnlockTalent(int Id)
        {
            return Talents.ContainsKey(Id);
        }

        public int GetLevelTalent(int Id)
        {
            if (!Talents.ContainsKey(Id))
            {
                return 0;
            }
            return Talents[Id];
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
        }

        public override void Fix()
        {
        }
    }
}
