using Gameplay;
using Manager;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.View;

namespace Common
{
    public abstract class GamePanel : Panel
    {
        private GameSceneManager m_SceneManager;

        public GameSceneManager SceneManager => m_SceneManager;
        public BaseSceneController SceneController => m_SceneManager.GetCurrentSceneController<BaseSceneController>();
        public BaseScenePresenter ScenePresenter => SceneController.ScenePresenter;
        public override void PostInit()
        {
            base.PostInit();
            m_SceneManager = GameSceneManager.Instance as GameSceneManager;
        }
    }
}
