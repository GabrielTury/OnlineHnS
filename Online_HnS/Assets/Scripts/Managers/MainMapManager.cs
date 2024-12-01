using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMapManager : NetworkBehaviour
{
    void Start()
    {
        /*
        if (NetworkManager.Singleton.StartHost())
        {
            print("Host started...");
            StartCoroutine(NetworkUIManager.instance.CheckPlayers());
        }
        else
        {
            print("Host could not be started...");
        }*/

        if (NetworkUIManager.instance.playerType == "Host")
        {
            NetworkManager.Singleton.StartHost();
            Debug.LogWarning("Host started");
        }
        else
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started");
        }
        StartCoroutine(NetworkUIManager.instance.CheckPlayers());
        Debug.LogWarning("Coroutine start");
    }

}
