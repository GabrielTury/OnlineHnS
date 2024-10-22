using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GroundEnemy : EnemyBase
{
#if UNITY_EDITOR
    [SerializeField] bool debugPath;
#endif
    public GameObject player;

    private Vector3[] path;

    public GameObject debugCube;

    private int currentPathIndex;

    [Button("CheckNavMesh")]
    protected override void CalculatePath(Vector3 playerPos)
    {
        Vector3[] newPath;
        Node ownNode = NavOperations.GetNearestNode(transform.position, NavMesh.allNodes);
        currentPathIndex = 0;
        newPath = NavOperations.CalculatePath(NavOperations.GetNearestNodeInPlane(playerPos, NavMesh.allNodes, ownNode.WorldPosition.y),
                                    ownNode,
                                    NavMesh.allNodes, false);

        if (path == null || NavOperations.CheckForPathDiff(newPath, path))
        {
            path = newPath;
        }
        else
            return;

        if (path.Length > 0)
        {
            if(moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(Move(path[0]));
        }
#if UNITY_EDITOR
        if (debugPath)
        {
            foreach (Vector3 node in path)
            {
                Instantiate(debugCube, node, Quaternion.identity);
            }
        }
#endif
    }

    protected override IEnumerator Move(Vector3 target)
    {
        while (path.Length > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 2);

            if (Vector3.Distance(transform.position, target) < .1f)
            {
                currentPathIndex++;
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        if (currentPathIndex < path.Length)
        {
            StartCoroutine(Move(path[currentPathIndex]));
        }
        else 
        {
            path = new Vector3[0];
        }
    }
}
