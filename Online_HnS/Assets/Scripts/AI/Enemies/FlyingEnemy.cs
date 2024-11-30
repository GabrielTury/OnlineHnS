using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{
#if UNITY_EDITOR
    [SerializeField] bool debugPath;
    List<GameObject> debugObjs = new List<GameObject>();
#endif

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
            if(path.Length > 1 )
                moveRoutine = StartCoroutine(Move(path[1]));
            else
                moveRoutine = StartCoroutine(Move(path[0]));
        }
#if UNITY_EDITOR
        if(debugPath)
        {
            if(debugObjs != null)
            foreach (GameObject go in debugObjs)
            {                
                Destroy(go);
            }
            if(debugObjs != null)
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
        if (goal == Vector3.zero)
            goal = target;

        while (path.Length > 0)
        {
            if (Vector3.Distance(transform.position, closestPlayer.position) < 6)
            {
                SetAIState(States.Attacking); 
                break;
            }

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

        Vector3 lookDir = goal;
        lookDir.y = transform.position.y;
        //@todo: Must rotate Smoothly instead of instantly
        transform.LookAt(lookDir/**Time.deltaTime*rotationSpeed*/);
    }

    protected override void SetAIState(States newState)
    {
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);

        //Stop Switch State
        switch (currentState)
        {
            case States.Idle:

                break;
            case States.Moving:
                StopCoroutine(moveRoutine);
                
                break;
            case States.Attacking:
                
                break;
            case States.Hit:

                break;
            case States.Death:
                break;
            default:
                break;
        }
        currentState = newState;
        switch (newState)
        {
            case States.Idle:

                break;
            case States.Moving:
                stateRoutine = StartCoroutine(UpdatePath());
                break;
            case States.Attacking:
                stateRoutine = StartCoroutine(Attack());
                break;
            case States.Hit:
                anim.SetTrigger("Hit");
                break;
            case States.Death:
                anim.SetTrigger("Death");
                break;
        }
    }

    protected override IEnumerator Attack()
    {
        anim.SetTrigger("Attack");
        GameObject bullet = AIManager.instance.GetBullet();
        bullet.SetActive(true);
        bullet.transform.position = transform.position;
        bullet.transform.LookAt(closestPlayer.position);
        yield return new WaitForSeconds(3);

        if (Vector3.Distance(transform.position, closestPlayer.position) < 6)
        {
            SetAIState(States.Attacking);
        }
        else
        {
            SetAIState(States.Moving);
        }
    }

    protected override IEnumerator TakeDamage()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator Die()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator Idle()
    {
        throw new System.NotImplementedException();
    }
}
