using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{
#if UNITY_EDITOR
    [SerializeField] bool debugPath;
    List<GameObject> debugObjs;
#endif
    public GameObject player;

    private Vector3[] path;

    public GameObject debugCube;

    private int currentPathIndex;

    [Button("CheckNavMesh")]
    protected override void CalculatePath(Node playerNode)
    {
        Vector3[] newPath;
        currentPathIndex = 0;
        newPath = NavOperations.CalculatePath(playerNode,
                                    ownNode,
                                    NavMesh.allNodes, true);

        path = newPath;

        if (path.Length > 0 )
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(Move(path[1]));
        }
#if UNITY_EDITOR
        if(debugPath)
        {
            foreach (GameObject go in debugObjs)
            {                
                Destroy(go);
            }
            debugObjs.Clear();
            foreach( Vector3 node in path )
            {
                
                debugObjs.Add(Instantiate(debugCube, node, Quaternion.identity));
            }
        }
#endif
    }

    protected override IEnumerator Move(Vector3 target)
    {
        if (goal == null)
            goal = target;

        while (path.Length > 0)
        {
            if (Vector3.Distance(transform.position, goal) < .1f)
            {
                currentPathIndex++;

                if (currentPathIndex < path.Length)
                {
                    goal = path[currentPathIndex];
                }
                else
                {
                    path = new Vector3[0];
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }

    }

    protected override void MoveToGoal()
    {
        transform.position = Vector3.MoveTowards(transform.position, goal, Time.deltaTime * 5);
    }
}
