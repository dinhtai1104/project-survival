using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.BattlePass.UI
{
    public class BattlePassUnlockable : MonoBehaviour, IUnlockable
    {
        public UIButtonFeature buttonUnlockable;
        public ValueConfigSearch LevelUnlockSubscription = new ValueConfigSearch("[BattlePass]Level_Unlock", "3");

        private void Awake()
        {
            GameSceneManager.Instance.PlayerData.ExpHandler.OnLevelChanged += ExpHandler_OnLevelChanged;
        }

        private void ExpHandler_OnLevelChanged(int from, int to)
        {
            buttonUnlockable.Init();
        }

        private void OnDestroy()
        {
            if (GameSceneManager.Instance != null)
                GameSceneManager.Instance.PlayerData.ExpHandler.OnLevelChanged -= ExpHandler_OnLevelChanged;
        }

        public bool Validate()
        {
            var playerData = GameSceneManager.Instance.PlayerData;

            return playerData.ExpHandler.CurrentLevel >= LevelUnlockSubscription.IntValue;
        }
    }
}
