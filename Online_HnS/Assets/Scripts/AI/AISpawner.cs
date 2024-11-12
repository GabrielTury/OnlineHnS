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
        SpawnEnemyServerRpc();
    }
    [ServerRpc]
    private void SpawnEnemyServerRpc()
    {
        GameObject spawned = Instantiate(SpawnEnemy, transform.position, Quaternion.identity);
        spawned.GetComponent<NetworkObject>().Spawn(true);
    }
}
