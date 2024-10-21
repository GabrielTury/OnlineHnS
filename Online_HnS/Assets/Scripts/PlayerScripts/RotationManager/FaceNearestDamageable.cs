using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FaceNearestDamageable : MonoBehaviour
{
    public float radius = 10f;

    private void LateUpdate()
    {
        FaceNearest();
    }

    private void FaceNearest()
    {
        // First, find all instances of gameObjects implementing the iDamageable interface
        IDamageable nearestTarget = FindNearestDamageable();
        if (nearestTarget != null)
        {
            Transform targetTransform = (nearestTarget as MonoBehaviour).transform;
            Vector3 direction = targetTransform.position - transform.position;
            direction.y = 0;

            if(direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public IDamageable FindNearestDamageable()
    {
        IDamageable[] allDamageables = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>().ToArray();
        IDamageable nearestOne = null;
        float minDistance = Mathf.Infinity;

        foreach(IDamageable damageable in allDamageables)
        {
            float distance = Vector3.Distance(transform.position, (damageable as MonoBehaviour).transform.position);
            if(distance < minDistance && distance <= radius)
            {
                minDistance = distance;
                nearestOne = damageable;
            }
        }
        return nearestOne;
    }
}
