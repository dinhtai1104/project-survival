using UnityEngine;

public class OpenStorePage
{
    private string iosId;
    private string androidId;

    public OpenStorePage(string iosId, string androidId)
    {
        this.iosId = iosId;
        this.androidId = androidId;
    }

    public void OpenPage()
    {
#if UNITY_EDITOR
        Debug.Log("Editor Not Support!");
#elif UNITY_ANDROID
        Application.OpenURL("market://details?id=" + androidId);
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + iosId);
#endif
    }
}