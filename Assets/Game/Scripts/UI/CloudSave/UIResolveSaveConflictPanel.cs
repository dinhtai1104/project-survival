using UnityEngine;


public class UIResolveSaveConflictPanel : UI.Panel
{
    public System.Action<CloudSave.ESaveType, CloudSave.SavePackage> onResolve;

    [SerializeField]
    private SaveInfoView localView, cloudView;
    public void OnSelect(CloudSave.ESaveType saveType,CloudSave.SavePackage save) 
    { 
        onResolve?.Invoke(saveType,save);
        Close();
    }

    public override void PostInit()
    {
    }
    public void SetUp(CloudSave.SavePackage localSave,CloudSave.SavePackage cloudSave)
    {
        localView.SetUp(CloudSave.ESaveType.Local,localSave,OnSelect);
        cloudView.SetUp(CloudSave.ESaveType.Cloud,cloudSave, OnSelect);
        Show();
    }
}
