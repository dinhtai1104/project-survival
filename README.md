First import project jenkins into your project
We need to notice file "CustomBuilder.cs"
```c#
public static class CustomBuilder
{
    private const string UserKeystore = "pokemash-sleep.keystore"; // keystore
    private const string KeyaliasName = "poke-sleep";
    private const string KeyaliasPass = "123456";
    private const string KeystorePass = "123456"; 

    private const string GoogleCredentialsId = ""; // Google Credentials to use jenkins auto publish internal to googleplay
    private const string FirebaseAndroidAppId = ""; // Firebase App
    private const string FirebaseIosAppId = ""; 
    private const string FirebaseCliToken = ""; // token login via CLI Firebase to push symbol to Firebase
}
```
we need to create branches on remote (branchBuild)
- ci/development // in development like unlock version
- ci/production // in production build aab. project will auto increase buildcode
- ci/internal // in production but in .apk file, and we can use this branch for ads test
- yourbranch logic

After you've done your logic. just merge your branches into branchBuild then push it to remote
website buid: http://183.80.51.158:8080/
