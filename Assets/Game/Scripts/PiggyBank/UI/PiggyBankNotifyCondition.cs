public class PiggyBankNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        try
        {
            var save = DataManager.Save.PiggyBank;
            if (save.CanClaim())
            {
                return true;
            }
        }
        catch (System.Exception e)
        {

        }
        return false;
    }
}