using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayGiftSaves : BaseDatasave
{
    public List<int> Saves = new List<int>();
    public bool Purchased = false;
    public PlayGiftSaves(string key) : base(key)
    {
        Saves = new List<int>();
    }

    public override void Fix()
    {
    }

    public bool IsClaimedPlay(int Session)
    {
        return Saves.Contains(Session);
    }
    public bool CanClaimedPlay(int Session)
    {
        var save = DataManager.Save.User;
        return save.NumberPlayGame >= Session;
    }

    public void Claim(int INDEX)
    {
        Saves.Add(INDEX);
        Save();
    }

    public void Purchase()
    {
        Purchased = true;
        Save();
    }
}
