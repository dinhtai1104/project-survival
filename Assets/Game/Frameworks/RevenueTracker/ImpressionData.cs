public struct ImpressionData
{
    public double revenue;
    public string platform;
    public string currencyCode;
    public string adNetwork;
    public string adUnit;
    public string adFormat;
    public string placement;

    public override string ToString()
    {
        return $"Revenue:{revenue}\tPlatform:{platform}\tCode:{currencyCode}\tNetwork:{adNetwork}\tUnit:{adUnit}\tFormat:{adFormat}\tPlacement:{placement}";
    }
}

