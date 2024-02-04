using System;
using UnityEngine;

[System.Serializable]
public enum ERarity : int
{
    Common = 0,
    UnCommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4,
    Ultimate,
}

public static class RarityExtension
{
    public static int GetMaxLevel(this ERarity r)
    {
        return DataManager.Base.EquipmentRarity.Dictionary[r].MaxLevel;
    }

    public static Color GetColor(ERarity rarity)
    {
        switch (rarity)
        {
            case ERarity.Common:
                break;
            case ERarity.UnCommon:
                return GameUtility.GameUtility.GetColor("4CFF42");
                break;
            case ERarity.Rare:
                return GameUtility.GameUtility.GetColor("42DAFF");
                break;
            case ERarity.Epic:
                return GameUtility.GameUtility.GetColor("F870FF");
                break;
            case ERarity.Legendary:
                return GameUtility.GameUtility.GetColor("FFFD96");
                break;
            case ERarity.Ultimate:
                return GameUtility.GameUtility.GetColor("FF5243");
                break;
        }
        return Color.white;
    }
}
