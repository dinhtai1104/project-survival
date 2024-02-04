
public class EndGameDungeonEventArgs : EndGameArgs
{
    public EDungeonEvent EventType;
    public override void Active()
    {
        var save = DataManager.Save.DungeonEvent.Saves[EventType];
        if (sessionSave.CurrentStage >= 29)
        {
            save.CurrentDungeon++;
        }
    }
}