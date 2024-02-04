using System;
using System.Collections.Generic;

[System.Serializable]
public class UserCloudModel
{
    public string deviceId;
    public string facebookId;
    public string googleId;
    public string appleId;
    public string version;
    public string created_at;
    public string updated_at;

    public string metadata;
}