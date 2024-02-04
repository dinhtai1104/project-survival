public class UIButtonConfirmEndGame : UIBaseButton
{
    public override void Action()
    {
        DataManager.Save.ClearSession();
        var playerData = GameSceneManager.Instance.PlayerData;
        var userSave = DataManager.Save.User;
        userSave.SetTryHero(EHero.None);
        playerData.Stats.ReplaceAllStatBySource(playerData.HeroDatas[userSave.Hero].heroStat, EStatSource.sourceHero);

        //load menu scene
        Game.Controller.Instance.LoadMenuScene();
    }
}
