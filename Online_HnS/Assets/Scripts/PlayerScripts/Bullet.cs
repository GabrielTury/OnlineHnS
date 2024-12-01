using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed = 10f;  // Speed of the bullet
    public float maxLifeSpan = 5f;  // Maximum lifespan before the bullet is destroyed

    private float lifeSpan;
    public GameObject playerGameObject;
    private void OnEnable()
    {
        lifeSpan = 0f;
        StartCoroutine(Movement());
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.CompareTag("Player") && other.GetComponent<IDamageable>() != null)
        {
            other.GetComponent<IDamageable>().Damage(10);
            Destroy(gameObject);

        }
    }

    IEnumerator Movement()
    {
        yield return new WaitForSeconds(0.05f);
        while (lifeSpan < maxLifeSpan)
        {
            lifeSpan += Time.deltaTime;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            yield return null;  // Wait for the next frame
        }

        // Destroy the bullet or deactivate it when the lifespan ends
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null && IsServer)
        {
            networkObject.Despawn();
        }
        else
        {
            gameObject.SetActive(false);  // If not using network, just deactivate
        }
    }
}
