using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;

    AISpawner[] spawners;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        spawners = FindObjectsOfType<AISpawner>();
    }

    [Button("Use all Spawners")]
    public void SpawnEnemies()
    {
        foreach(AISpawner spawner in spawners)
        {
            spawner.SpawnEnemyFunc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SendDamageToServerRpc(ulong id)
    {
        foreach(NetworkObject p in PlayerManager.instance.players)
        {
            if(p.OwnerClientId == id)
            {
                p.GetComponent<IDamageable>().Damage(10);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
