using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AISpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject SpawnEnemy;

    [SerializeField]
    private int type;
    
    private int spawnAmount = 1;

    private GameObject enemy;

    private void Start()
    {
        
    }    
    [Button("Spawn Enemy in Server")]    
    public GameObject SpawnEnemyFunc()
    {
        SpawnEnemyServerRpc();
        return enemy;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc()
    {
        GameObject spawned = Instantiate(SpawnEnemy, transform.position, Quaternion.identity);
        spawned.GetComponent<NetworkObject>().Spawn(true);
        spawned.GetComponent<EnemyBase>().StartBehaviour();
        enemy = spawned;
    }

    private void OnDrawGizmos()
    {
        if(type == 0)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
