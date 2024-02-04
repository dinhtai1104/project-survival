using Assets.Game.Scripts.DungeonWorld.Save;
using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.UI.Home_UI.Minimap
{
    public class UIMapBossVisual : MonoBehaviour
    {
        [SerializeField] private Image avatarBossImg;
        [SerializeField] private GameObject notiObject;
        [SerializeField] private GameObject claimedObject;
        private DungeonWorldStageSave save;

        public void SetData(DungeonWorldStageSave save)
        {
            try
            {
                var entity = DataManager.Base.DungeonWorld.Get(save.Dungeon).Get(save.Stage);

                var path = $"boss_{entity.Index + 1}_map_{save.Dungeon + 1}";
                var sprite = ResourcesLoader.Instance.GetSprite(AtlasName.AvatarBoss, path);
                avatarBossImg.sprite = sprite;
            }catch(System.Exception e)
            {
                Logger.Log(save == null);
                Logger.Log(save.Dungeon+ " " + save.Stage);
                Logger.Log(DataManager.Base.DungeonWorld.Get(save.Dungeon).Get(save.Stage) == null);
                Logger.LogError(e);
            }
            this.save = save;
            Init();
        }

        private void Init()
        {
            notiObject.SetActive(false);

            var dungeonSave = DataManager.Save.Dungeon;

            claimedObject.SetActive(save.IsClaimed);

            if (dungeonSave.IsDungeonCleared(save.Dungeon))
            {
                if (!save.IsClaimed)
                {
                    notiObject.SetActive(true);
                }
            }
            else
            {
                if (dungeonSave.CurrentDungeon >= save.Dungeon)
                {
                    if (dungeonSave.BestStage >= save.Stage)
                    {
                        if (!save.IsClaimed)
                        {
                            notiObject.SetActive(true);
                        }
                    }
                }
            }
        }

        public void OnClicked()
        {
            bool canReward = false;
            var dungeonSave = DataManager.Save.Dungeon;
            if (dungeonSave.IsDungeonCleared(save.Dungeon))
            {
                if (!save.IsClaimed)
                {
                    canReward = true;
                }
            }
            else
            {
                if (dungeonSave.CurrentDungeon >= save.Dungeon)
                {
                    if (dungeonSave.BestStage >= save.Stage)
                    {
                        if (!save.IsClaimed)
                        {
                            canReward = true;
                        }
                    }
                }
            }
            PanelManager.CreateAsync(AddressableName.UIDungeonWorldPanel).ContinueWith(t=>
            {
                var menu = PanelManager.Instance.GetPanel<UIMainMenuPanel>();
                menu.HideByTransitions().Forget();

                (t as UIDungeonWorldPanel).Show(save.Dungeon);
                t.onClosed += () =>
                {
                    PanelManager.Instance.GetPanel<UIMainMenuPanel>().ShowByTransitions();
                    //Init();
                    menu.UpdateMap();
                };
            }).Forget();
        }
    }
}
