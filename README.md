ABOUT PROJECT

I love create simply architecture system, can scalable for any project

This project base on game Pickle Pete, , you can search this game, this project start from 3/2024-5/2024, I gapped for a long time, and I will back in soon

This project contains:

- Base game RPG for any project, it's clear and simply, scalable like: Attribute System, Equipment/Rune/Socket System, Battle System, Enemy Spawn System, Movement System, Buff,...
- Dependency Injection for easy use inject any object in scene optimize
- Database use BG Database, Datasave use SaveGamePro
- Easy implement new feature as new Services by implement interface ```IService``` from ```com.sparkle.core```
- Auto create services by interface
- Event Dispatcher by object by ```IEventMgr```
- And many feature implement in furture 




I used Jenkins service to build this project, and you can use this service for any project, it's work perfectly
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
