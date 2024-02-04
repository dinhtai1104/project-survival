using Assets.Game.Scripts.BaseFramework.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts._Services
{
    public class AchievementService : Service
    {
        [SerializeField] private AchievementQuestTable table;
        [SerializeField] private AchievementQuestSaves save;

        private ExpHandler expHandler;
        public override void OnInit()
        {
            base.OnInit();
            table = DataManager.Base.AchievementQuest;
            save = DataManager.Save.Achievement;
        }
        public override void OnStart()
        {
            base.OnStart();
            expHandler = GameSceneManager.Instance.PlayerData.ExpHandler;
            expHandler.OnLevelChanged += ExpHandler_OnLevelChanged;
            if (Architecture.Get<AchievementService>().GetSave(EAchievement.AccountLevel).Progress < expHandler.CurrentLevel)
            {
                Architecture.Get<AchievementService>().SetProgress(EAchievement.AccountLevel, expHandler.CurrentLevel);
            }
        }

        public override void OnDispose()
        {
            base.OnDispose();
            expHandler.OnLevelChanged -= ExpHandler_OnLevelChanged;
        }

        private void ExpHandler_OnLevelChanged(int from, int to)
        {
            if (Architecture.Get<AchievementService>().GetSave(EAchievement.AccountLevel).Progress < to)
            {
                Architecture.Get<AchievementService>().SetProgress(EAchievement.AccountLevel, to);
            }
        }

        public void SetProgress(EAchievement type, int progress)
        {
            save.SetProgress(type, progress);
        }

        public void IncreaseProgress(EAchievement type)
        {
            save.IncreaseProgress(type);
        }

        public void ReceiveAchievement(EAchievement type)
        {
            save.ReceiveAchievement(type);
        }

        public bool IsReceiveAchievementIndex(EAchievement type, int Index)
        {
            return save.IsReceiveAchievementIndex(type, Index);
        }

        public AchievementQuestSave GetSave(EAchievement type)
        {
            return save.GetSave(type);
        }
        public bool CanReceive()
        {
            return save.CanReceive();
        }
        public bool CanReceive(EAchievement type)
        {
            return save.CanReceive(type);
        }

        public bool IsLootAll(EAchievement type)
        {
            return save.IsLootAll(type);
        }
    }
}
