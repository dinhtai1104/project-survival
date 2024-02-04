using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleListenner : MonoBehaviour
{
    public static bool VISIBLE_CHECK = false;
    IOnVisible listenner;
    private void OnEnable()
    {
        Check();
    }
    async UniTask Check()
    {
        listenner = GetComponentInParent<IOnVisible>();
        await UniTask.Delay(500);
        if ( listenner != null)
        {
            try
            {

                listenner.OnVisible(GetComponent<Renderer>().isVisible);
            }
            catch(System.Exception e)
            {
                listenner.OnVisible(true);
            }
        }
    }
    private void OnDisable()
    {
        listenner = null;
    }
    private void OnDestroy()
    {
        listenner = null;
    }
    private void OnBecameInvisible()
    {
        if(listenner!=null)
            listenner.OnVisible(false);
    }
    private void OnBecameVisible()
    {
        if(listenner!=null)
            listenner.OnVisible(true);
    }
}
