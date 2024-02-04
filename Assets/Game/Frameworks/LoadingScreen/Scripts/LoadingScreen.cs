using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen instance;
    public static LoadingScreen Instance { get => instance; set => instance = value; }

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetProgress(0);
    }
    [SerializeField]
    private TMPro.TextMeshProUGUI progressText;
    [SerializeField]
    private UnityEngine.UI.Image progressBar;


    public void SetProgress(float progress)
    {
        progressBar.fillAmount = progress;
        progressText.text = Mathf.RoundToInt(progress * 100 )+"%";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        GetComponent<Animator>().SetTrigger("Close");
    }
    public void Deactive()
    {
        gameObject.SetActive(false);
        Instance = null;
        //Addressables.ReleaseInstance(gameObject);
        Destroy(gameObject);
    }
   
}
