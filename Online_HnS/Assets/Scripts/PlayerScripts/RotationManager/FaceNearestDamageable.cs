using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FaceNearestDamageable : MonoBehaviour
{
    public float radius = 10f;
    IDamageable[] allDamageables;
    List<Transform> d_Transforms;

    private void Awake()
    {
        d_Transforms = new List<Transform>();
    }

    private void OnEnable()
    {
        GameEvents.Damageable_Spawn += GetDamageableTransform;
    }

    private void OnDisable()
    {
        GameEvents.Damageable_Spawn -= GetDamageableTransform;
    }

    private void Start()
    {
    }

    private void LateUpdate()
    {
        FaceNearest();
    }

    private void FaceNearest()
    {
        // First, find all instances of gameObjects implementing the iDamageable interface
        Transform nearestTarget = FindNearestDamageable();
        if (nearestTarget != null)
        {
            Vector3 direction = nearestTarget.position - transform.position;
            direction.y = 0;

            if(direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    //private void GetDamageableArray()
    //{
    //    allDamageables = FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>().ToArray();
    //}

    public Transform FindNearestDamageable()
    {
        Transform nearestOne = null;
        float minDistance = Mathf.Infinity;

        foreach(Transform damageable in d_Transforms)
        {
            float distance = Vector3.Distance(transform.position, damageable.position);
            if(distance < minDistance && distance <= radius)
            {
                minDistance = distance;
                nearestOne = damageable;
            }
        }
        return nearestOne;
    }

    void GetDamageableTransform(Transform objectTransform)
    {
        d_Transforms.Add(objectTransform);
        print(objectTransform.gameObject.name);
    }
}
