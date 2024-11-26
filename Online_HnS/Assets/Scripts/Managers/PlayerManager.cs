using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public List<NetworkObject> players {  get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this);

        players = new List<NetworkObject>();
        //player = FindFirstObjectByType<PlayerStatus>().gameObject;
    }
    private void OnEnable()
    {
        GameEvents.Player_Spawn += PlayerSpawned;
    }

    private void OnDisable()
    {
        GameEvents.Player_Spawn -= PlayerSpawned;
    }

    private void PlayerSpawned(NetworkObject p)
    {
        UpdatePlayerListServerRpc(p);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerListServerRpc(NetworkObject p)
    {
        if(NetworkManager.Singleton.IsHost)
        {
            players.Add(p);
        }
        else
        {
            NetworkObject[] objs = FindObjectsOfType<NetworkObject>();
            foreach(var obj in objs)
            {
                if (obj.CompareTag("Player"))
                {
                    players.Add(p);
                }
            }
        }
        Debug.Log(players.Count);
    }
}
