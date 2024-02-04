using UnityEngine;

public enum EDamage
{
    Normal,
    Poison,
    Flame,
    Critital,
    Frost,
    HeadShot,
    Missed,
    Bleeding,
    Shock
}

public static class DamageTypeExtension
{
    public static Color _normal = Color.white;
    public static Color _poison = new Color(147f / 255, 229f / 255, 30f / 255, 255f / 255);
    public static Color _flame = new Color(255f / 255, 153f / 255, 102f / 255, 255f / 255);
    public static Color _critital = new Color(255 / 255f, 154f / 255, 0, 255f / 255);

    public static Color GetColor(this EDamage type)
    {
        switch (type)
        {
            case EDamage.Poison:
                return _poison;
            case EDamage.Flame:
                return _flame;
            case EDamage.Critital:
                return _critital;
        }

        return Color.white;
    }
}