[System.Serializable]
public class BuffSave
{
    public int Level;
    public int StageBuff;
    public bool CanActiveAgain = true;

    public void LevelUp()
    {
        Level++;
    }
}