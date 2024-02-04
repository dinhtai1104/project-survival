using System;
[System.Serializable]
public class AttributeStatModifier
{
    public StatKey StatKey;
    public StatModifier Modifier;

    public AttributeStatModifier() { }

    public AttributeStatModifier(string content, float mul = 1f)
    {
        var split = content.Split(';');
        if (split.Length == 3)
        {
            // Dmg;PercentAdd;0.2
            Enum.TryParse(split[0], out StatKey);
            Enum.TryParse(split[1], out EStatMod statMod);
            float value = float.Parse(split[2]) * mul;

            Modifier = new StatModifier(statMod, value);
        }
        else
        {
            // Dmg;0.3
            Enum.TryParse(split[0], out StatKey);
            float value = float.Parse(split[1]) * mul;

            Modifier = new StatModifier(EStatMod.Flat, value);
        }
    }

    public AttributeStatModifier(StatKey statKey, StatModifier modifier)
    {
        StatKey = statKey;
        Modifier = modifier;
    }

    public override string ToString()
    {
        return I2Localize.GetLocalize("Affix/Base_" + StatKey + "_" + Modifier.Type, Modifier.Value);
    }

    public AttributeStatModifier Clone()
    {
        return new AttributeStatModifier(this.StatKey, new StatModifier(Modifier.Type, Modifier.Value));
    }
}