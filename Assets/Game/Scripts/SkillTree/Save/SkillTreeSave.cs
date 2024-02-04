[System.Serializable]
public class SkillTreeSave
{
    public int Level;
    public int Index;
    public bool IsClaimed;

    public SkillTreeSave(int level, int index, bool isClaimed)
    {
        Level = level;
        Index = index;
        IsClaimed = isClaimed;
    }

    public SkillTreeSave() { }
}