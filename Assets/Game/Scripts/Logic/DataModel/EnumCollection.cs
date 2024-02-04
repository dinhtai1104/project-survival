using System;
using System.Collections.Generic;

public static class EnumCollection
{
    public static List<EHero> AllHeroes = new List<EHero>();

    static EnumCollection()
    {
        foreach (var item in (EHero[])Enum.GetValues(typeof(EHero)))
        {
            if (item == EHero.None) continue;
            AllHeroes.Add(item);
        }
    }
}