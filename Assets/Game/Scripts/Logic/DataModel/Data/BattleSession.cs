using System.Collections.Generic;

[System.Serializable]
public class BattleSession 
{
    public int Dungeon;
    public int Stage;
    public GameMode Mode;
    public List<int> Buff;
    public string Reroll = null;
}