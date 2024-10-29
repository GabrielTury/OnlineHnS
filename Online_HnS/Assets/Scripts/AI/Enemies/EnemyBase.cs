using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public int health { get; private set; }

    protected Coroutine moveRoutine;

    protected Node ownNode;

    protected Node lastPlayerNode;

    protected Vector3 goal;
    
    public void Damage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Death();
        }
    }
    protected void Death()
    {
        Destroy(gameObject);
    }

    protected abstract void CalculatePath(Node playerNode);    

    protected abstract IEnumerator Move(Vector3 target);
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdatePath());
        GameEvents.OnDamageableSpawned(transform);
    }

    // Update is called once per frame
    void Update()
    {
        MoveToGoal();
    }

    protected IEnumerator UpdatePath()
    {
        while (true)
        {
            Vector3 playerPos = PlayerManager.instance.player.transform.position;
            ownNode = NavOperations.GetNearestNode(transform.position, NavMesh.allNodes);
            Node playerNode = NavOperations.GetNearestNodeInPlane(playerPos, NavMesh.allNodes, ownNode.WorldPosition.y);

            if (playerNode != lastPlayerNode || lastPlayerNode == null)
            {
                lastPlayerNode = playerNode;
                CalculatePath(playerNode);
            }
            for (int i = 0; i < 3; i++)
            {
                yield return null;
            }
        }
    }

    protected abstract void MoveToGoal();
}
