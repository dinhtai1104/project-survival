using Cysharp.Threading.Tasks;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuthentication : MonoBehaviour
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Firebase Logout")]
    static void WipeData()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

#endif

    public static FirebaseAuthentication Instance;
    protected Firebase.Auth.FirebaseAuth auth;

    public bool isReady = false;
    public Firebase.Auth.FirebaseUser User => auth == null ? null: auth.CurrentUser;

    public string linkedAccountID, userName;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            FirebaseManager.onInit -= Init;
            FirebaseManager.onInit += Init;
            if (FirebaseManager.Instance != null && FirebaseManager.Instance.isReady && auth == null)
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
            Logger.Log("AUTH INIT " + (Firebase.Auth.FirebaseAuth.DefaultInstance == null));
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.StateChanged -= AuthStateChanged;
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
            isReady = true;
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
       

    }
    void OnDestroy()
    {
        if (auth == null) return;
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
   

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (User == null) return;
        Logger.Log("AuthStateChanged " + User.UserId +" "+User.DisplayName);
    }

  
    public async UniTask<FirebaseUser> SignInAsGuest()
    {
        Logger.Log("SignInAsGuest");
        try
        {
            if (User != null)
            {
                return User;
            }
            UniTask<AuthResult> task = auth.SignInAnonymouslyAsync().AsUniTask();
            var authResult=await task;
            if (task.Status==UniTaskStatus.Canceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return null;
            }
            if (task.Status==UniTaskStatus.Faulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Status);
                return null;
            }

            Debug.LogFormat("User signed in successfully: {0} ({1})",
                User.DisplayName, User.UserId);
            return User;

        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public async UniTask<FirebaseUser> SignInWithEmail(string email,string password)
    {
        try
        {
            if (User != null)
            {
                return User;
            }
            UniTask<AuthResult> task = auth.SignInWithEmailAndPasswordAsync(email,password).AsUniTask();
            var authResult = await task;
            if (task.Status == UniTaskStatus.Canceled)
            {
                Debug.LogError("SignInWithEmail was canceled.");
                return null;
            }
            if (task.Status == UniTaskStatus.Faulted)
            {
                Debug.LogError("SignInWithEmail encountered an error: " + task.Status);
                return null;
            }

            Debug.LogFormat("User signed in successfully: {0} ({1})",
                User.DisplayName, User.UserId);
            return User;

        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    
#if UNITY_ANDROID

    public async UniTask<FirebaseUser> SignInWithGoogle(Google.GoogleSignInUser googleAccount)
    {
        Logger.Log("FIREBASE SignInWithGoogle "+googleAccount.DisplayName);
        string token = googleAccount.IdToken;
        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(token, null);

        var user= await SignInWithCredential(credential);
        if (user != null)
        {
            string name = "Player";
            if (!string.IsNullOrEmpty(googleAccount.DisplayName))
            {
                name = googleAccount.DisplayName;
            }
            else if (!string.IsNullOrEmpty(googleAccount.GivenName))
            {
                name = googleAccount.GivenName;
            }
            await user.UpdateUserProfileAsync(new UserProfile() { DisplayName = name }).AsUniTask();
        }
        return user;
    }

#endif
    public async UniTask<FirebaseUser> SignInWithFacebook(FacebookSDK.User acc)
    {
        string token = acc.accessToken;
        //Logger.Log("TOKEN:" + token +" "+acc.userId+" "+acc.userName);
        Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(token);

        var user = await SignInWithCredential(credential);
        if (user != null)
        {
            await user.UpdateUserProfileAsync(new UserProfile() { DisplayName = acc.userName }).AsUniTask();
        }

        return  user;
    }



    public async UniTask<FirebaseUser> SignInWithApple(AppleUser appleUser)
    {
        Logger.Log("FIREBASE SignInWithApple " + appleUser.ToString());
        Firebase.Auth.Credential credential = Firebase.Auth.OAuthProvider.GetCredential("apple.com", appleUser.identityToken, appleUser.rawNonce, appleUser.authorizationCode);

        var user = await SignInWithCredential(credential);
        if (user != null)
        {
            await user.UpdateUserProfileAsync(new UserProfile() { DisplayName = appleUser.displayName }).AsUniTask();
        }
        return user;
    }


    public async UniTask<FirebaseUser> SignInWithCredential(Firebase.Auth.Credential credential)
    {
        Logger.Log("FIREBASE SignInWithCredential");

        if (User == null)
        {
            return null;
        }
        Logger.Log("FIREBASE LinkWithCredentialAsync "+credential.Provider.ToString()+" "+credential.IsValid());
        try
        {
            var result = await User.LinkWithCredentialAsync(credential).AsUniTask();
            return result.User;

        }catch(System.Exception e)
        {
            Logger.LogError(e);
           
            if (e.InnerException.Message.Contains("This credential is already associated with a different user account."))
            {
                return await ReauthenticateUser(credential);
            }
            else
            {
                return null;
            }
        }
    }

    async UniTask<FirebaseUser> ReauthenticateUser(Credential credential)
    {
        try
        {

            if (User != null && User.IsAnonymous)
            {
                await DeleteAccount(User);
            }


            var user = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
            Logger.Log("SignInWithCredentialAsync. New: " + user.UserId + " " + user.IsAnonymous);

            
            return user;

        }
        catch (System.Exception e)
        {
            Logger.Log(e);
            return null;
        }
    }

    public async UniTask<bool> DeleteAccount(FirebaseUser user)
    {
        try
        {

            Logger.Log("DELETE ACCOUNT: " + user.IsAnonymous + " " + user.UserId);
            await user.DeleteAsync();
            return true;
        }
        catch(System.Exception e)
        {
            Logger.LogError(e);

            return false;
        }
    }

    public void Clear()
    {
        linkedAccountID = string.Empty;
        userName = string.Empty;
    }
    public bool IsLoggedIn()
    {
        return auth != null && User!=null;
    }

    public void LogOut()
    {
        auth.SignOut();
        Clear();
    }
}

