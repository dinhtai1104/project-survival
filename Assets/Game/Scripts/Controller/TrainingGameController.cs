using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingGameController : GameController
{
    //public override async UniTask Initialize()
    //{
    //    //
    //    LevelLoading.Instance.Close();



    //    await PrepareLevel();
    //    await StartBattle();


    //}

    //public override async UniTask StartBattle()
    //{
    //    await StartBattle();
    //    testGun = DataManagement.DataManager.Instance.userData.inventory.SelectedMainGun.id;
    //    testSkin = DataManagement.DataManager.Instance.userData.inventory.SelectedOutfit.id;
    //}

    //public override async UniTask Finish(Character lastPlayerAlive = null, int matchRank = -1)
    //{
    //    if (isFinished) return;
    //    isFinished = true;

    //}
    //public int respawnTime = 2000;


    //protected override void OnDie(ActorBase pointBase, ActorBase damageSource)
    //{
    //    if (isFinished) return;
    //    base.OnDie(pointBase, damageSource);
    //}



    //[SerializeField]
    //string testGun,testSkin;
    //private void OnGUI()
    //{
    //    GUILayout.Space(Screen.height / 10);
    //    testGun = GUILayout.TextField(testGun, GUILayout.Width(200));
    //    testSkin = GUILayout.TextField(testSkin, GUILayout.Width(200));
    //    if (GUILayout.Button("UPDATE"))
    //    {
    //        WeaponBase playerWeapon = Sheet.SheetDataManager.Instance.gameData.gameConfig.GetGun(testGun);
    //        player.SetWeapon(playerWeapon);


    //        player.AnimationHandler.SetSkin(testSkin);



    //    }
    //}
    public override EnemySpawnHandler GetEnemySpawnHandler()
    {
        throw new System.NotImplementedException();
    }

    public override LevelBuilderBase GetLevelBuilder()
    {
        throw new System.NotImplementedException();
    }

    public override ActorBase GetMainActor()
    {
        throw new System.NotImplementedException();
    }

    public override UniTask PrepareLevel(int room=-1)
    {
        throw new System.NotImplementedException();
    }

    public override UniTask StartBattle()
    {
        throw new System.NotImplementedException();
    }
}
