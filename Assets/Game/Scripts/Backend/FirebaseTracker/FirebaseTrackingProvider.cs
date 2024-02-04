using Firebase.Analytics;

public class FirebaseTrackingProvider : ITrackingProvider
{
    public bool IsReady { private set; get; }

    public FirebaseTrackingProvider()
    {
        IsReady = true;
    }

    public ITrackingProvider SetUserId(string id)
    {
        FirebaseAnalytics.SetUserId(id);
        return this;
    }

    public ITrackingProvider SetUserProperty(string id, string value)
    {
        FirebaseAnalytics.SetUserProperty(id, value);
        return this;
    }

    public IEvent NewEvent(string id)
    {
        return new FirebaseEvent(id);
    }
}