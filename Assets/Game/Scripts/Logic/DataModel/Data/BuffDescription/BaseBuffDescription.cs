[System.Serializable]
public abstract class BaseBuffDescription : IBuffDescription
{
    protected BuffEntity entity;
    protected int level;
    public abstract string GetDescription();

    public void SetData(BuffEntity entity, int level)
    {
        this.entity = entity;
        this.level = level;
    }
}