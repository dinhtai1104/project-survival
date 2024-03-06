using Common;
using Gameplay.Data;
using MenuScene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay.Dungeon
{
    public class UIDungeonEndGamePanel : UIGameplayEndGame
    {

        public void MenuOnClicked()
        {
            SceneManager.LoadSceneAsync<MenuSceneController>();
        }
    }
}
