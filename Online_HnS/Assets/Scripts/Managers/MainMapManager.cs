using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMapManager : NetworkBehaviour
{
    void Start()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            print("Host started...");
            StartCoroutine(NetworkUIManager.instance.CheckPlayers());
        }
        else
        {
            print("Host could not be started...");
        }
    }

}
