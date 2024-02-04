using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVolumnHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      

        gameObject.SetActive(DataManager.Save.General.IsHDR);
    }

}
