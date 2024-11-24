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
        players.Add(p);
        Debug.Log(players.Count);
    }
}
