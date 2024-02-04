[System.Serializable]
public abstract class BaseAffix
{
    public virtual string DescriptionKey { get; set; } = "Value";
    public virtual string GetDescription()
    {
        return I2Localize.GetLocalize("Affix/" + DescriptionKey);
    }
    public virtual void OnEquip(IStatGroup stats)
    {
    }
    public virtual void OnUnEquip()
    {
    }
    public virtual void OnAddToItem(EquipableItem item)
    {
    }
    public virtual void OnRemoveFromItem(EquipableItem item)
    {
    }
    public virtual void OnHitTarget(IActor owner, IActor target)
    {
    }
    public virtual void OnKillTarget(IActor owner, IActor target)
    {
    }
}