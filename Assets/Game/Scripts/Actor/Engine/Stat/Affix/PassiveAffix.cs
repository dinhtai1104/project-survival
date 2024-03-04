namespace Engine
{
    [System.Serializable]
    public class PassiveAffix : BaseAffix
    {
        public PassiveAffix(string Affix) : base()
        {
            DescriptionKey = Affix;
        }
    }
}