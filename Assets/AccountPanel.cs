using Cysharp.Threading.Tasks;
#if UNITY_ANDROID
using Google;
using Google.Impl;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AccountPanel : UI.Panel
{
    Action onSignedIn;
    Action onFailed;
     [SerializeField]
    private GameObject appleBtn, googleBtn;
    public override void PostInit()
    {
      
#if UNITY_EDITOR

        appleBtn.SetActive(true);
        googleBtn.SetActive(true);
#elif UNITY_ANDROID
      
        appleBtn.SetActive(false);
        googleBtn.SetActive(true);
#elif UNITY_IOS
        appleBtn.SetActive(true);
        googleBtn.SetActive(false);

#endif
    }

    public void SetUp(System.Action onSignedIn=null,Action onFailed=null)
    {
        this.onSignedIn = onSignedIn;
        this.onFailed = onFailed;
        Show();
    }


    public void SignInWithFacebook()
    {
        WaitingPanel.Show(() =>
        {
            Logger.Log("START SIGN IN FACEBOOK");
            var signInHandler = new SignIn.FacebookSignIn();
            signInHandler.SignIn().ContinueWith(user =>
            {
                Logger.Log("START LINK FIREBASE FACEBOOK " + (user != null));
                if (user != null)
                {
                    FirebaseAuthentication.Instance.SignInWithFacebook(user).ContinueWith(fbUser =>
                    {
                        if (fbUser == null)
                        {
                            OnSignInFailed();
                        }
                        else
                        {
                            OnSignedIn();
                        }
                    }).Forget();
                }
                else
                {
                    OnSignInFailed();
                }

            }).Forget();
        });
    }
   
    public void SignInGoogle()
    {
#if UNITY_ANDROID
        WaitingPanel.Show(()=> 
        {
            Logger.Log("START SIGN IN GOOGLE");
            var signInHandler = new SignIn.GoogleSignIn();
            signInHandler.SignIn().ContinueWith(user =>
            {
                Logger.Log("START LINK FIREBASE GOOGLE "+(user!=null));
                if (user != null)
                {
                    FirebaseAuthentication.Instance.SignInWithGoogle(user).ContinueWith(fbUser =>
                    {
                        if (fbUser == null)
                        {
                            OnSignInFailed();
                        }
                        else
                        {
                            OnSignedIn();
                        }
                    }).Forget();
                }
                else
                {
                    OnSignInFailed();
                }

            }).Forget();
        });

#endif

        
        
    }
    public void SignInApple()
    {
#if UNITY_IOS || UNITY_EDITOR
        WaitingPanel.Show(() =>
        {
            Logger.Log("START SIGN IN APPLE");
            var signInHandler = new SignIn.AppleSignIn();
            signInHandler.SignIn().ContinueWith(user =>
            {
                Logger.Log("START LINK FIREBASE APPLE " + (user != null));
                if (user != null)
                {
                    FirebaseAuthentication.Instance.SignInWithApple(user).ContinueWith(appleUser =>
                    {
                        if (appleUser == null)
                        {
                            OnSignInFailed();
                        }
                        else
                        {
                            OnSignedIn();
                        }
                    }).Forget();
                }
                else
                {
                    OnSignInFailed();
                }

            }).Forget();
        });
#endif
    }

    void OnSignInFailed()
    {
        WaitingPanel.Hide();
        UI.PanelManager.CreateAsync<UIMessagePanel>(AddressableName.UIMessagePanel).ContinueWith(panel =>
        {
            panel.SetUp("Something is wrong. Try again later");
            panel.Show();
            panel.onClosed = () =>
            {
                onFailed?.Invoke();
            };
        }).Forget();
    }

    void OnSignedIn()
    {
        WaitingPanel.Hide();
        Close();

        UI.PanelManager.CreateAsync<UIMessagePanel>(AddressableName.UIMessagePanel).ContinueWith(panel => 
        {
            panel.SetUp("Your account has been linked!");
            panel.Show();
            panel.onClosed = () => 
            {
                onSignedIn?.Invoke();
            };

            var user = FirebaseAuthentication.Instance.User;
            DataManager.Save.User.PlayerName = string.IsNullOrEmpty(user.DisplayName)?"Player":user.DisplayName;
            DataManager.Save.SaveData();


        }).Forget();

    }
    
}


namespace SignIn
{
    public abstract class SignInHandler<T>
    {
        public abstract  UniTask<T> SignIn();
    }

#if UNITY_ANDROID
    public class GoogleSignIn : SignInHandler<GoogleSignInUser>
    {
        private const string WEB_CLIENT_ID = "110968391299-ssi4ra78pma6hnbpg6dijki6nb9ns29l.apps.googleusercontent.com";

        public GoogleSignIn()
        {
            if (Google.GoogleSignIn.Configuration == null)
            {
                Google.GoogleSignIn.Configuration = new Google.GoogleSignInConfiguration
                {
                    RequestIdToken = true,
                    WebClientId = WEB_CLIENT_ID
                };
            }
        }

        public override async UniTask<GoogleSignInUser> SignIn()
        {
            //await UniTask.Delay(250);
            //try
            //{
            //    if (Google.GoogleSignIn.Configuration != null)
            //    {
            //        Google.GoogleSignIn.Configuration = new Google.GoogleSignInConfiguration
            //        {
            //            RequestIdToken = true,
            //            WebClientId = WEB_CLIENT_ID
            //        };
            //    }
            //}
            //catch(System.Exception e) {
            //    Debug.LogError("ERROR: 1 "+e);
            //}
            try
            {

                UniTask<GoogleSignInUser> task = Google.GoogleSignIn.DefaultInstance.SignIn();
                var user = await task;

                return user;
            }
            catch (System.Exception e)
            {
                Debug.LogError("error2 "+e.ToString());
                return null;
            }
        }

    }
#endif
    public class FacebookSignIn : SignInHandler<FacebookSDK.User>
    {

        public FacebookSignIn()
        {
         
        }

        public override async UniTask<FacebookSDK.User> SignIn()
        {
            Debug.Log("SIGN IN WITH Facebook");
            try
            {
                return await FacebookSDK.Login();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return null;
            }
        }

    }


#if UNITY_IOS || UNITY_EDITOR
    public class AppleSignIn : SignInHandler<AppleUser>
    {

        public AppleSignIn()
        {

        }

        public override async UniTask<AppleUser> SignIn()
        {
            Debug.Log("SIGN IN WITH Apple");
            try
            {
                return await new AppleSignInHandler().SignInWithApple();
            }
            catch(System.Exception e)
            {

                UI.PanelManager.CreateAsync<UIMessagePanel>(AddressableName.UIMessagePanel).ContinueWith(panel =>
                {
                    panel.SetUp(e.Message + " " + e.InnerException);
                }).Forget();
                return null;
            }
            
        }

    }
#endif


}

public class AppleUser
{
    public string displayName;
    public string identityToken;
    public string authorizationCode;
    public string rawNonce;

    public AppleUser()
    {
    }

    public AppleUser(string displayName, string identityToken, string authorizationCode, string rawNonce)
    {
        this.displayName = displayName;
        this.identityToken = identityToken;
        this.authorizationCode = authorizationCode;
        this.rawNonce = rawNonce;
    }
}