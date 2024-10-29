using System.Collections;
using System.Collections.Generic;
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

            if (path.Length > 1)
                moveRoutine = StartCoroutine(Move(path[1]));
            else
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
        if(goal == Vector3.zero)
            goal = target;

        anim.SetBool("Walk", true);
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

        Vector3 direction = goal - transform.position;

        // Create the rotation we want to be in
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3);
    }

    protected override void SetAIState(States newState)
    {
        StopCoroutine(stateRoutine);

        switch (currentState)
        {
            case States.Idle:

                break;
            case States.Moving:
                anim.SetBool("Walk", false);
                break;
            case States.Attacking:

                break;
            case States.Hit:

                break;
            case States.Death:

                break;
        }

        switch (newState)
        {
            case States.Idle:

                break;
            case States.Moving:
                StartCoroutine(UpdatePath());
                break;
            case States.Attacking:

                break;
            case States.Hit:

                break;
            case States.Death:

                break;
        }
    }

    protected override IEnumerator Attack()
    {
        anim.SetTrigger("Attack");
        throw new System.NotImplementedException();
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
