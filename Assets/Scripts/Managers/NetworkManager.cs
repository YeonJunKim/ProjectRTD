using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkManager : MonoBehaviour
{
    public static NetworkManager S;

    private void Awake()
    {
        if(S == null)
            S = this;
    }



    public void OnTowerFire()
    {

    }
}
