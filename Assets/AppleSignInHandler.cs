#if UNITY_IOS || UNITY_EDITOR
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AppleSignInHandler:IBatchUpdate
{
    private const string AppleUserIdKey = "AppleUserId";

    private IAppleAuthManager appleAuthManager;


    public AppleSignInHandler()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }

    }

    ~AppleSignInHandler()
    {

    }



    public async UniTask<AppleUser> SignInWithApple()
    {
        if (this.appleAuthManager == null) return null;
        UpdateSlicer.Instance.RegisterSlicedUpdate(this, UpdateSlicer.UpdateMode.Always);
        var tcs = new TaskCompletionSource<AppleUser>();
        var rawNonce = GenerateRandomString(32);
        var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

        var loginArgs = new AppleAuthLoginArgs(
            LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
            nonce);
      


        this.appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                Logger.Log("LOGIN SUCCESS ");
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    var displayName = (appleIdCredential.FullName==null || string.IsNullOrEmpty(appleIdCredential.FullName.GivenName)) ?"AppleUser":appleIdCredential.FullName.GivenName;
                    var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

                    tcs.SetResult(new AppleUser(displayName,identityToken,authorizationCode, rawNonce));
                }
                else
                {
                    tcs.SetResult(null);
                }
            },
            error =>
            {
                Logger.Log("LOGIN FAILED " + error.Code + " " + error.LocalizedDescription + " " + error.LocalizedFailureReason);

                tcs.SetResult(null);
                // Something went wrong
            });


        AppleUser result = await tcs.Task.AsUniTask();

        UpdateSlicer.Instance.DeregisterSlicedUpdate(this);

        return result;
    }



    private static string GenerateSHA256NonceFromRawNonce(string rawNonce)
    {
        var sha = new SHA256Managed();
        var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
        var hash = sha.ComputeHash(utf8RawNonce);

        var result = string.Empty;
        for (var i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");
        }

        return result;
    }
    private static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            throw new Exception("Expected nonce to have positive length");
        }

        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
        var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
        var result = string.Empty;
        var remainingLength = length;

        var randomNumberHolder = new byte[1];
        while (remainingLength > 0)
        {
            var randomNumbers = new List<int>(16);
            for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
            {
                cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                randomNumbers.Add(randomNumberHolder[0]);
            }

            for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
            {
                if (remainingLength == 0)
                {
                    break;
                }

                var randomNumber = randomNumbers[randomNumberIndex];
                if (randomNumber < charset.Length)
                {
                    result += charset[randomNumber];
                    remainingLength--;
                }
            }
        }

        return result;
    }

    public void BatchUpdate()
    {
        if (appleAuthManager != null)
        {
            appleAuthManager.Update();
        }
    }

    public void BatchFixedUpdate()
    {
    }
}

#endif
