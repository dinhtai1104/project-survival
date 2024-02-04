using Firebase;
using Firebase.Crashlytics;
using UnityEngine;

public class FirebaseCrashlyticsInit : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

        FirebaseManager.onInit -= OnInit;
        FirebaseManager.onInit += OnInit;
        DontDestroyOnLoad(gameObject);
    }
    void OnInit()
    {
        try
        {
            Crashlytics.ReportUncaughtExceptionsAsFatal = false;
            Debug.Log("INIT crashlystic");
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
       
    }

}