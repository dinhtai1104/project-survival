using Cysharp.Threading.Tasks;
using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FacebookSDK : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init();
        }
        DontDestroyOnLoad(gameObject);
    }
    public static async UniTask<User> Login()
    {
        Debug.Log("LOGIN WITH FACEBOOK");
        var perms = new List<string>() { "public_profile" };

        var tcs = new TaskCompletionSource<User>();




        FB.LogInWithReadPermissions(perms, (result) =>
        {
            if (FB.IsLoggedIn)
            {
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                //Debug.Log(aToken.UserId+" "+aToken.TokenString);
                FB.API("/me?fields=id,name", HttpMethod.GET, (graphResult) =>
                {
#if UNITY_EDITOR
                    foreach(var field in graphResult.ResultDictionary)
                    {
                        Logger.Log(field.Value + ": " + field.Value);
                    }
#endif
                    tcs.SetResult(new User(aToken.TokenString,aToken.UserId, result.Error == null ? graphResult.ResultDictionary["name"].ToString() : ""));
                });
            }
            else
            {
                Debug.Log("User cancelled login");
                tcs.SetCanceled();
            }
        });

        return await tcs.Task.AsUniTask();


    }



    [System.Serializable]
    public class User
    {
        public string accessToken;
        public string userId;
        public string userName;

        public User(string accessToken, string userId, string userName)
        {
            this.accessToken = accessToken;
            this.userId = userId;
            this.userName = userName;
        }
        public override string ToString()
        {
            return $"{accessToken} {userId} {userName}";
        }
    }
}
