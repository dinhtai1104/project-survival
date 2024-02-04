public static class I2Localize
{
    public static string GetLocalize(string key)
    {
        if (I2.Loc.LocalizationManager.TryGetTranslation(key, out var trans))
        {
            return trans;
        }
        return key;
    }
    public static string GetLocalize(string key, params object[] param)
    {
        if (I2.Loc.LocalizationManager.TryGetTranslation(key, out var trans))
        {
            trans = string.Format(trans, param);
            return trans;
        }
        return key;
    }


    // Some Common key

    public static string I2_NoticeNotEnough => GetLocalize("Notice/NotEnough") + " ";


    public static string GetLocalize(this EHero hero)
    {
        return I2Localize.GetLocalize($"Hero_Name/{hero}");
    }
}