using Assets.Game.Scripts.Talent.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Talent
{
    public interface ITalentService
    {
        bool HasUnlockTalent(int Id);
        List<TalentEntity> GetRollTalent(int amount);
        void ClaimTalent(int Id);
        void ApplyTalent(IStatGroup statGroup);
        int GetLevel(int id);
    }
}
