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

    private void Start()
    {
        GameEvents.OnDamageableSpawned(transform);        
    }

    [Button("CheckNavMesh")]
    protected override void CalculatePath(Vector3 playerPos)
    {
        Vector3[] newPath;
        currentPathIndex = 0;
        newPath = NavOperations.CalculatePath(NavOperations.GetNearestNode(playerPos, NavMesh.allNodes),
                                    NavOperations.GetNearestNode(transform.position, NavMesh.allNodes),
                                    NavMesh.allNodes, true);

        if (path == null || NavOperations.CheckForPathDiff(newPath, path))
        {
            path = newPath;
        }
        else
            return;


        if (path.Length > 0 )
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(Move(path[0]));
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
        Debug.Log("Called Move");
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
