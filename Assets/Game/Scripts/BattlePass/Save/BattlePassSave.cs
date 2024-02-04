[System.Serializable]
public class BattlePassSave
{
    public int Id;
    public int Level;
    public bool FreeClaimed = false;
    public bool PremiumClaimed = false;

    public void Save()
    {
        DataManager.Save.BattlePass.Save();
    }
}