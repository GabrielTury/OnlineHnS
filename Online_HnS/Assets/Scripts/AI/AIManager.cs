using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AIManager : NetworkBehaviour
{
    public static AIManager instance;

    public List<GameObject> bullets;
    public GameObject bullet;

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

    private void OnEnable()
    {
        GameEvents.Player_Spawn += Bullets;
    }

    private void OnDisable()
    {
        GameEvents.Player_Spawn -= Bullets;
    }
    public void Bullets(NetworkObject n)
    {

        if (NetworkManager.Singleton.IsHost)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject g = Instantiate(bullet);
                g.GetComponent<NetworkObject>().Spawn(true);
                bullets.Add(g);
                g.SetActive(false);
            }
        }
    }

    public GameObject GetBullet()
    {
        foreach(GameObject b in bullets)
        {
            if(!b.activeInHierarchy)
            {
                return b;                
            }
        }

        GameObject g = Instantiate(bullet);
        g.GetComponent<NetworkObject>().Spawn(true);
        bullets.Add(g);
        g.SetActive(false);
        return g;
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
                Debug.Log(p.name);
                IDamageable Iface = p.GetComponentInChildren<IDamageable>();
                if(Iface != null)
                {
                    Iface.Damage(10);
                    Debug.Log(p.name + "TookDamage");
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
