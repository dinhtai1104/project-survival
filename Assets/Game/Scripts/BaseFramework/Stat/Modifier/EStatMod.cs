public enum EStatMod
{
    Flat,
    PercentAdd,
    PercentMul,
    FlatMul,
    Percent
}


public static class EStatModExtension
{
    public static string GetRomanTxt(this EStatMod mod)
    {
        switch (mod)
        {
            case EStatMod.PercentAdd:
                return "%";
            case EStatMod.PercentMul:
                return "%";
        }
        return "";
    }
}