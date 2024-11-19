using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AISpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject SpawnEnemy;

    [SerializeField]
    private int spawnAmount;

    private void Start()
    {
        
    }    
    [Button("Spawn Enemy in Server")]    
    public void SpawnEnemyFunc()
    {
        SpawnEnemyServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc()
    {
        GameObject spawned = Instantiate(SpawnEnemy, transform.position, Quaternion.identity);
        spawned.GetComponent<NetworkObject>().Spawn(true);
    }
}
