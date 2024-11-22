using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour, IDamageable
{
    private int playerMaxHealth = 100;
    public int playerCurrentHealth;
    [SerializeField]
    private PlayerCombat playerCombat;



    public override void OnNetworkSpawn()
    {
        playerCombat = gameObject.GetComponent<PlayerCombat>();
    }

    void Start()
    {
        playerCurrentHealth = playerMaxHealth;
    }

    [ContextMenu("Damage")]
    public void Damage(int damage)
    {
        playerCurrentHealth -= damage;
        if (playerCurrentHealth > 0)
        {
            playerCombat.HandleDamage();
        }
        else
        {
            // playerCombat.HandleDeath();
        }
    }

}
