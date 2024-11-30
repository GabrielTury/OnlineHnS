using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AIBullet : NetworkBehaviour
{
    private Collider col;
    private Vector3 goal;
    private float timer;

    private void OnEnable()
    {
        timer = 0;
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 7)
        {
            gameObject.SetActive(false);
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * 10);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            //@todo: remove GetComponent to improve cpu performance
            ulong id = GetComponent<Collider>().GetComponent<NetworkBehaviour>().OwnerClientId;
            AIManager.instance.SendDamageToServerRpc(id);
            //collider.GetComponent<IDamageable>().Damage(10);
        }

        gameObject.SetActive(false);
    }
}
