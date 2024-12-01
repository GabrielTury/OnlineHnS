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

    private void OnEnable()
    {
        GameEvents.EnemyKilled += RemoveEnemy;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= RemoveEnemy;
    }
    private void SpawnEnemies()
    {
        foreach (AISpawner s in spawners)
        {
            spawnedEnemies.Add(s.SpawnEnemyFunc());
        }

        foreach (GameObject s in wallLimits)
        {
            s.SetActive(true);
        }
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
            foreach (GameObject s in wallLimits)
            {
                s.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    { 
        if(other.CompareTag("Player"))
        {
            used = true;
            SpawnEnemies();
        }
    }
}
