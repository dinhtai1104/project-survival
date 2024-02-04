using System.Collections.Generic;

public abstract class EndGameArgs
{
    public List<LootParams> LootParams { get; set; }
    public DungeonSessionSave sessionSave;
    public abstract void Active();
}
