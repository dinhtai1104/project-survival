using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.Subscription.Services;
using Assets.Game.Scripts.Talent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.BaseFramework.Architecture
{
    public class GameArchitecture : Architecture
    {
        public override void Inject()
        {
            Register<ShortTermMemoryService>();
            Register<NotifiQueueService>();
            Register(DataManager.Instance);
            Register<TalentService>();
            Register(EnergyService.Instance);
            Register<BattlePassService>();
            Register<PiggyBankServices>();
            Register<SubscriptionService>();
            Register<QuestService>();
            Register<AchievementService>();
            Register<PlayGiftService>();
            //Register<SkillTreeService>(); // Bo Skill tree

            Register<IAPService>();
            Register<AdService>();

            // Set Energy Capacity
            var modifierEnergyTalent = new StatModifier(EStatMod.Flat, Get<TalentService>().GetValueAttribute(StatKey.EnergyStorage));
            Get<EnergyService>().AddCapacityModifier(modifierEnergyTalent);

            DataManager.Save.OnLoaded();
            DataManager.Save.FixData();
        }
    }
}
