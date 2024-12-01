using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
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
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
        StartCoroutine(NetworkUIManager.instance.CheckPlayers());
    }

}
