using UnityEngine;

public class DungeonEventUnlockable : MonoBehaviour, IUnlockable
{
    public UIButtonFeature buttonUnlockable;
    public ValueConfigSearch LevelUnlock = new ValueConfigSearch("[DungeonEvent]Level_Unlock", "3");

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

        return playerData.ExpHandler.CurrentLevel >= LevelUnlock.IntValue;
    }
}
