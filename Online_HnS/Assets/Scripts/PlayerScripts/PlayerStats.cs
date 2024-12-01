using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour, IDamageable
{
    private int playerMaxHealth = 30;
    public int playerCurrentHealth;
    [SerializeField]
    private PlayerCombat playerCombat;


    [SerializeField] private Vector3 hostSpawnPosition = new Vector3(2.5f, 1.58f, 0);
    [SerializeField] private Vector3 clientSpawnPosition = new Vector3(-2.5f, 1.58f, 0);


    public override void OnNetworkSpawn()
    {
        
        playerCombat = gameObject.GetComponent<PlayerCombat>();

        if (IsOwner)
        {
            if (IsServer)
            {
                transform.position = hostSpawnPosition;
            }
            else if (IsClient)
            {
                transform.position = clientSpawnPosition;
            }
        }
    }

    void Start()
    {
        playerCurrentHealth = playerMaxHealth;
    }

    [ContextMenu("Damage")]
    public void Damage(int damage)
    {
        if (!IsOwner) { return; }


        playerCurrentHealth -= damage;
        Debug.Log(playerCurrentHealth);

        if(!playerCombat.isDead)
        {
            if (playerCurrentHealth > 0)
            {
                playerCombat.HandleDamage();
            }
            else
            {
                playerCombat.HandleDeath();
            }
        }
        


        if (IsServer)
        {
            GameEvents.Player_Damaged(damage, 0);
        }
        else
        {
            GameEvents.Player_Damaged(damage, 1);

        }

        
    }

}
