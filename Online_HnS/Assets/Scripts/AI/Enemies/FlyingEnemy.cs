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
                                    chunk, true);

        path = newPath;

        if (path.Length > 0 )
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            if (path.Length > 1 && currentState == States.Moving)
                moveRoutine = StartCoroutine(Move(path[1]));
            else if (currentState == States.Moving)
                moveRoutine = StartCoroutine(Move(path[0]));

            if (moveRoutine != null)
                stateRoutine = moveRoutine;
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
                Node playerNode = NavOperations.GetNearestNodeInPlane(closestPlayer.position, chunk, ownNode.WorldPosition.y);
                if (playerNode == lastPlayerNode)
                {
                    CalculatePath(playerNode);
                }
                break;
            case States.Attacking:
                stateRoutine = StartCoroutine(Attack());
                break;
            case States.Hit:
                anim.SetTrigger("Hit");
                stateRoutine = StartCoroutine(TakeDamage());
                break;
            case States.Death:
                anim.SetTrigger("Death");
                break;
        }
    }

    protected override IEnumerator Attack()
    {
        anim.SetTrigger("Attack");        
        yield return new WaitForSeconds(3);
        if (currentState != States.Attacking) yield return null;

        if (Vector3.Distance(transform.position, closestPlayer.position) < 6)
        {
            SetAIState(States.Attacking);
        }
        else
        {
            SetAIState(States.Moving);
        }
    }

    public void ShootBulet()
    {
        GameObject bullet = AIManager.instance.GetBullet();
        bullet.SetActive(true);
        bullet.transform.position = transform.position;
        bullet.transform.LookAt(closestPlayer.position);
    }

    protected override IEnumerator TakeDamage()
    {
        yield return new WaitForSeconds(1);
        if (currentState == States.Hit)
            SetAIState(States.Moving);
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
