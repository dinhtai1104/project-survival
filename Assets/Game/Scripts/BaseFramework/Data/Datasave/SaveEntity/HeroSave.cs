[System.Serializable]
public class HeroSave
{
    public bool IsUnlocked;
    public EHero Type;
    public int Level;
    public int Star;
    public void Fix()
    {
    }
    public void Unlocked()
    {
        IsUnlocked = true;
    }

    public void UpgradeStar()
    {
        Star++;
    }
    public void UpgradeLevel()
    {
        Level++;
    }
}
