using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{
    public GameObject player;

    private Vector3[] path;

    public GameObject debugCube;

    private int currentPathIndex;

    private void Start()
    {
        GameEvents.OnDamageableSpawned(transform);        
    }

    [Button("CheckNavMesh")]
    protected override void CalculatePath()
    {
        currentPathIndex = 0;
        path = NavOperations.CalculatePath(NavOperations.GetNearestNode(player.transform.position, NavMesh.allNodes),
                                    NavOperations.GetNearestNode(transform.position, NavMesh.allNodes),
                                    NavMesh.allNodes);
        if(path.Length > 0 )
        {
            StartCoroutine(Move(path[0]));
        }
        foreach( Vector3 node in path )
        {
            Instantiate(debugCube, node, Quaternion.identity);
        }
     }

    protected override IEnumerator Move(Vector3 target)
    {
        while(path.Length > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 2);

            if(Vector3.Distance(transform.position, target) < .1f )
            {
                currentPathIndex++;
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        if(currentPathIndex < path.Length)
        {
            StartCoroutine(Move(path[currentPathIndex]));
        }
    }

}
