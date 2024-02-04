using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseFireStore : MonoBehaviour
{


    public static FirebaseFireStore Instance;
    FirebaseFirestore DB;
    public bool isReady = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            FirebaseManager.onInit -= Init;
            FirebaseManager.onInit += Init;
            if (FirebaseManager.Instance != null && FirebaseManager.Instance.isReady)
            {
                Init();
            }

        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    void Init()
    {
        try
        {
            DB = FirebaseFirestore.DefaultInstance;
            isReady = true;
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
      

    }
    void OnDestroy()
    {
      
    }

    public async UniTask AddData(string collectionId, string userId,string userCollectionId,string documentId, Dictionary<string,string> datas)
    {
        try
        {
            Logger.Log($"ADD DATA {collectionId}/{userId}/{userCollectionId}/{documentId}");
            DocumentReference docRef = DB.Collection(collectionId).Document(userId).Collection(userCollectionId).Document(documentId);
#if UNITY_EDITOR
            foreach(var data in datas)
            {
                Logger.Log(data.Key + ": " + data.Value);
            }
#endif
            await docRef.SetAsync(datas).ContinueWithOnMainThread(task =>
            {
                Debug.Log("Added data to the alovelace document in the users collection.");
                datas.Clear();
            });
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
    }
  

    /// <summary>
    /// Load data from cloud
    /// </summary>
    /// <param name="collectionId">Database Collection ID.</param>
    /// <param name="documentId">UserId</param>
    /// 
    Dictionary<string, string> result=new Dictionary<string, string>();
    public async UniTask<Dictionary<string,string>> GetData(string collectionId, string userId, string userCollectionId, string documentId)
    {
        result.Clear();
        Logger.Log("GetData  " + collectionId);

        DocumentReference usersRef = DB.Collection(collectionId).Document(userId).Collection(userCollectionId).Document(documentId);
        var snapshot = await usersRef.GetSnapshotAsync();

        Debug.Log(String.Format("User: {0}", snapshot.Id));

        Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
        if (documentDictionary != null)
        {
            foreach (var data in documentDictionary)
            {
#if UNITY_EDITOR
                Logger.Log($"{data.Key}:{data.Value}");
#endif
                result.Add(data.Key, data.Value.ToString());
            }
        }
        Debug.Log("Read all data from the users collection.");
        return result;
    }
    //[SerializeField]
    //TestUserSave save = new TestUserSave() { id = "123456", userName = "test name", level = 100 };
    //private void OnGUI()
    //{
    //    //if (GUILayout.Button("New save " + save.id, GUILayout.Width(200), GUILayout.Height(100)))
    //    //{
    //    //    save.id = UnityEngine.Random.Range(0, 1000000).ToString();
    //    //    AddData("User", save.id, save).ContinueWith(() => { Logger.Log("PUSH DATA SUCCESS"); }).Forget();
    //    //}

    //    //if (GUILayout.Button("SAVE " + save.id + " =>" + save.level, GUILayout.Width(200), GUILayout.Height(100)))
    //    //{
    //    //    save.level++;
    //    //    AddData("User", save.id, save).ContinueWith(() => { Logger.Log("PUSH DATA SUCCESS"); }).Forget();
    //    //}
    //    //if (GUILayout.Button("LOAD", GUILayout.Width(200), GUILayout.Height(100)))
    //    //{
    //    //    GetData("UserData", FirebaseAuthentication.Instance.User.UserId);
    //    //}
    //}

}

[System.Serializable]
public class TestUserSave
{
    public string id;
    public string userName;
    public int level;

    public TestUserSave()
    {
    }
}