using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.DataModel.Noti
{
    public class NotifiQueueCheckDungeonOfferFlashSale : NotifiQueueData
    {
        public override bool IsShowOnly => false;
        public NotifiQueueCheckDungeonOfferFlashSale(string type) : base(type)
        {
        }

        public async override UniTask Notify()
        {
            var user = DataManager.Save.User;
            var dungeonSave = DataManager.Save.Dungeon;
            var dungeonOffer = DataManager.Save.Offer.OfferDungeon;
            int dungeon = -1;
            var achieve = Architecture.Get<AchievementService>();

            int currenDuneonClear = achieve.GetSave(EAchievement.ClearDungeon).Progress - 1;

            for (int i = 0; i < dungeonSave.DungeonShowUnlocked.Count; i++)
            {
                var offer = dungeonOffer.GetItem(i);
                if (offer == null || offer.BoughtCount > 0)
                {
                    continue;
                }
                else
                {
                    dungeon = i;
                    // break;
                }
            }
            if (dungeon == -1) dungeon = 0;

            if (user.IsFirstDie && !user.CheckFirstDie)
            {
                user.CheckFirstDie = true;
                user.Save();
            }
            else
            {
                if (currenDuneonClear == -1) return;
            }

            dungeon = PlayerPrefs.GetInt("LastDungeon", 0);
            var mode = PlayerPrefs.GetString("LastMode", "Normal");
            if (mode != "Normal") return;
            var offere = dungeonOffer.GetItem(dungeon);
            if (offere == null || offere.BoughtCount > 0)
            {
                return;
            }

            var ui = await PanelManager.CreateAsync<UIFlashSaleDungeonOfferPanel>(AddressableName.UIFlashSaleDungeonOfferPanel);
            ui.Show(dungeon);
            var close = false;
            ui.onClosed += () => close = true;
            await UniTask.WaitUntil(() => close);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }
}
