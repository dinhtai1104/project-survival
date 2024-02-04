using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class IntroLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject skipBtn;
    void Start()
    {
        
    }
    public void OnEnd()
    {
        Skip();
    }
    bool isSkip = false;
    public void Skip()
    {
        if (!isSkip)
        {
            isSkip = true;
            LoadScene();
        }
    }
    async UniTask LoadScene()
    {
        skipBtn.SetActive(false);
     
    }
}
