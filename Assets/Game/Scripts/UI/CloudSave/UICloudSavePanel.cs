using Cysharp.Threading.Tasks;

public class UICloudSavePanel : UI.Panel
{
    public SaveInfoView view;
    public void OpenSave()
    {
      
    }

    public override void PostInit()
    {
       
    }
    public void SetUp(CloudSave.SavePackage currentSave)
    {
        view.SetUp(CloudSave.ESaveType.Local,currentSave, (type,save)=> 
        {
            WaitingPanel.Show(() => 
            {
                CloudSave.Controller.Instance.Save().ContinueWith(status =>
                {
                    WaitingPanel.Hide();

                    SetUp(currentSave);
                }).Forget();
            });
        });
        Show();
    }
}
