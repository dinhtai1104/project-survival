public class BaseBuffObject : CharacterObjectBase
{
    private EBuff buffType;
    public EBuff BuffType => buffType;
    public void SetBuff(EBuff buffType)
    {
        this.buffType = buffType;
    }
}