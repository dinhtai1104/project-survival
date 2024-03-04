namespace Engine
{
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
    }
}