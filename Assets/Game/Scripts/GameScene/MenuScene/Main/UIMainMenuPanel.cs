using Assets.Game.Scripts.Dungeon;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.View;
using UnityEngine.InputSystem;

namespace MenuScene.Main
{
    public class UIMainMenuPanel : GamePanel
    {
        public void PlayOnClicked()
        {
            SceneManager.LoadSceneAsync<DungeonSceneController>();
        }
    }
}
