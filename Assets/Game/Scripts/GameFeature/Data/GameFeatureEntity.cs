using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class GameFeatureEntity
{
    public EFeature Feature;
    public RequireData RequireUnlock;

    public GameFeatureEntity(BGEntity e)
    {
        Enum.TryParse(e.Get<string>("Feature"), out Feature);
        RequireUnlock = new RequireData(e.Get<string>("Require"));
    }
}