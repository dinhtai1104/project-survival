[System.Serializable]
public class BuffAtrData
{
    public int IdBuff;
    public EBuff Type;
    public int Level;
    public GroupModifier StatRefer;
    public int StageStartBuff = 0;
    private BuffEntity entity;
    public int PlayBuffCount = 0;

    public BuffAtrData() { }

    public BuffAtrData(int IdBuff, EBuff Type, int stageStart)
    {
        this.IdBuff = IdBuff;
        this.Type = Type;
        StatRefer = new GroupModifier();
        this.StageStartBuff = stageStart;
        entity = DataManager.Base.Buff.Dictionary[Type];
        Equip();
    }

    public float GetValueStat(StatKey StatKey)
    {
        return StatRefer.GetModifier(StatKey).Value;
    }

    public void SetLevel(int Level)
    {
        this.Level = Level;

        StatRefer.RemoveAllMod();
        var item = entity.LevelCard[Level];
        item.Add(StatRefer);
    }

    public void Equip()
    {
        Level = 0;

        StatRefer.RemoveAllMod();
        if (entity.LevelCard.ContainsKey(Level))
        {
            var item = entity.LevelCard[Level];
            item.Add(StatRefer);
        }
    }

    public void LevelUp()
    {
        StatRefer.RemoveAllMod();
        Level++;
        if (Level >= entity.LevelMaxCard - 1)
        {
            Level = entity.LevelMaxCard - 1;
        }
        var item = entity.LevelCard[Level];
        item.Add(StatRefer);
    }
}