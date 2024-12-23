using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Encounter : NetworkBehaviour
{
    [SerializeField]
    private List<AISpawner> spawners = new List<AISpawner>();

    List<GameObject> spawnedEnemies;

    [SerializeField]
    private int cicles;

    [SerializeField]
    private GameObject[] wallLimits;

    private bool used = false;

    private void Awake()
    {
        spawnedEnemies = new List<GameObject>();
    }
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= RemoveEnemy;
    }
    private void SpawnEnemies()
    {
        foreach (AISpawner s in spawners)
        {
            s.isInEncounter = true;
            spawnedEnemies.Add(s.SpawnEnemyFunc());
        }

        /*foreach (GameObject s in wallLimits)
        {
            s.SetActive(true);
        }*/
    }

    private void RemoveEnemy(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        SpawnCicles();
    }
    private void SpawnCicles()
    {
        if(cicles > 0 && spawnedEnemies.Count <= 0)
        {
            SpawnEnemies();
            cicles--;
        }
        else if(cicles <= 0)
        {
            //DespawnServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void DespawnServerRpc()
    {
        foreach (GameObject s in wallLimits)
        {
            s.SetActive(false);
            s.GetComponent<NetworkObject>().Despawn();
            Destroy(s);
        }
    }
    private void OnTriggerEnter(Collider other)
    { 
        if(other.CompareTag("Player"))
        {
            if(!used)
            {
                GameEvents.EnemyKilled += RemoveEnemy;
                used = true;
                SpawnEnemies();

            }
        }
    }
}
