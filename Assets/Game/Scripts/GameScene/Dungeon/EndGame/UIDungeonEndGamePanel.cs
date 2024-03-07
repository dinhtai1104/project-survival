using Common;
using Gameplay.Data;
using MenuScene;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Dungeon
{
    public class UIDungeonEndGamePanel : UIGameplayEndGame
    {
        [SerializeField] protected Text m_TextEndGame;
        public new DungeonGameplaySessionSave GameSession => base.GameSession as DungeonGameplaySessionSave;

        protected override void Setup()
        {
        }

        //
        public void MenuOnClicked()
        {
            SceneManager.LoadSceneAsync<MenuSceneController>();
        }
    }
}
