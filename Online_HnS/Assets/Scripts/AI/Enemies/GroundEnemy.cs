using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundEnemy : EnemyBase
{
#if UNITY_EDITOR
    [SerializeField] bool debugPath;
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
                                    NavMesh.allNodes, false);

        path = newPath;

        if (path.Length > 0)
        {
            if(moveRoutine != null)
                StopCoroutine(moveRoutine);

            if (path.Length > 1 && currentState == States.Moving)
                moveRoutine = StartCoroutine(Move(path[1]));
            else if (currentState == States.Moving)
                moveRoutine = StartCoroutine(Move(path[0]));

            if(moveRoutine != null)
                stateRoutine = moveRoutine;
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
        if(goal == Vector3.zero)
            goal = target;

        anim.SetBool("Walk", true);
        while (path.Length > 0)
        {
            if (Vector3.Distance(transform.position, closestPlayer.transform.position) < 2f)
            {
                SetAIState(States.Attacking);
                //Debug.Log("Close Enough");
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
        transform.position = Vector3.MoveTowards(transform.position, goal, Time.deltaTime * moveSpeed);                

        Vector3 lookDir = goal;
        lookDir.y = transform.position.y;
        //@todo: Must rotate Smoothly instead of instantly
        transform.LookAt(lookDir/**Time.deltaTime*rotationSpeed*/);
    }

    protected override void SetAIState(States newState)
    {
        if(stateRoutine != null)
            StopCoroutine(stateRoutine);

        //Stop Switch State
        switch (currentState)
        {
            case States.Idle:

                break;
            case States.Moving:
                StopCoroutine(moveRoutine);
                anim.SetBool("Walk", false);
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
                Node playerNode = NavOperations.GetNearestNodeInPlane(closestPlayer.position, NavMesh.allNodes, ownNode.WorldPosition.y);
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
        yield return null;        
    }
    #region AttackBehaviour
    /// <summary>
    /// Contain logic for attack, called from animation event handler
    /// </summary>
    public void AttackBehaviour()
    {
        Collider[] hitObjs = Physics.OverlapBox((transform.position + new Vector3(0, 2, 0)) + (transform.forward * 1.5f), new Vector3(1.5f, 1.5f, 1.5f));
        if(hitObjs.Length > 0 )
        {
            foreach(Collider collider in hitObjs)
            {
                if (collider.CompareTag("Player"))
                {
                    //@todo: remove GetComponent to improve cpu performance
                    ulong id = collider.GetComponent<NetworkBehaviour>().OwnerClientId;
                    AIManager.instance.SendDamageToServerRpc(id);
                    //collider.GetComponent<IDamageable>().Damage(10);
                }
            }
        }
    }
    /// <summary>
    /// Logic for end of attack, called from animation event handler
    /// </summary>
    public void AttackEnd()
    {
        if (currentState != States.Attacking) return;

        if (Vector3.Distance(transform.position, closestPlayer.transform.position) < 2f)
        {
            SetAIState(States.Attacking);
            Vector3 lookDir = goal;
            lookDir.y = transform.position.y;            
            transform.LookAt(lookDir);
            Debug.Log("Close Enough");
        }
        else
        {
            SetAIState(States.Moving);
        }
    }
    #endregion
    protected override IEnumerator TakeDamage()
    {
        yield return new WaitForSeconds(1);
        if(currentState == States.Hit)
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

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((transform.position + new Vector3(0,2,0)) + (transform.forward * 1.5f), new Vector3(1.5f, 1.5f, 1.5f));
    }*/
}
