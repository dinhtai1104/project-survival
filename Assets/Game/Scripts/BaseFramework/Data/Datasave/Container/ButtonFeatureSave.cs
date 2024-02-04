using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ButtonFeatureSave : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<EFeature, bool> Features;

    public ButtonFeatureSave(string key) : base(key)
    {
        Features = new Dictionary<EFeature, bool>();
        foreach (var feature in (EFeature[])Enum.GetValues(typeof(EFeature)))
        {
            if (!Features.ContainsKey(feature))
            {
                Features.Add(feature, false);
            }
        }
    }

    public void Unlock(EFeature f)
    {
        Features[f] = true;
        Save();
    }
   
    public bool IsUnlock(EFeature f)
    {
        return Features[f];
    }

    public override void Fix()
    {
        foreach (var feature in (EFeature[])Enum.GetValues(typeof(EFeature)))
        {
            if (!Features.ContainsKey(feature))
            {
                Features.Add(feature, false);
            }
        }
    }
}
