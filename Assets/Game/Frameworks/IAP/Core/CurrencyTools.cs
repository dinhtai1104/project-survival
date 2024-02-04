using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public static class CurrencyTools
{
    private static IDictionary<string, string> Map;

    static CurrencyTools()
    {
        Map = CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Where(c => !c.IsNeutralCulture)
            .Select(culture =>
            {
                try
                {
                    return new RegionInfo(culture.Name);
                }
                catch
                {
                    return null;
                }
            })
            .Where(ri => ri != null)
            .GroupBy(ri => ri.ISOCurrencySymbol)
            .ToDictionary(x => x.Key, x => x.First().CurrencySymbol);
    }

    public static bool TryGetCurrencySymbol(string isoCurrencySymbol, out string symbol)
    {
        if (Map == null)
        {
            Map = CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Where(c => !c.IsNeutralCulture)
            .Select(culture =>
            {
                try
                {
                    return new RegionInfo(culture.Name);
                }
                catch
                {
                    return null;
                }
            })
            .Where(ri => ri != null)
            .GroupBy(ri => ri.ISOCurrencySymbol)
            .ToDictionary(x => x.Key, x => x.First().CurrencySymbol);
        }
        return Map.TryGetValue(isoCurrencySymbol, out symbol);
    }
}