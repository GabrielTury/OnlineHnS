using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    #region Enums
    protected enum States
    {
        Idle,
        Moving,
        Attacking,
        Hit,
        Death


    }

    protected States currentState;
    #endregion
    public int health { get; private set; }

    public float moveSpeed { get; protected set; } = 5f;

    public float rotationSpeed { get; protected set; } = 4;

    protected Coroutine stateRoutine;

    protected Coroutine moveRoutine;
    protected bool bIsMoving;

    protected Node ownNode;
    protected Node lastNode;

    protected Node lastPlayerNode;

    protected Vector3 goal;
    #region Components
    protected Animator anim;
    #endregion
    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Death());
        }
        else
        {
            anim.SetTrigger("Hit");
        }
    }
    protected IEnumerator Death()
    {
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    protected abstract void CalculatePath(Node playerNode);    

    protected abstract IEnumerator Move(Vector3 target);
    protected abstract IEnumerator Attack();
    protected abstract IEnumerator TakeDamage();
    protected abstract IEnumerator Die();
    protected abstract IEnumerator Idle();
    // Start is called before the first frame update

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();    
    }

    void Start()
    {        
        GameEvents.OnDamageableSpawned(transform);
    }
    [Button("Start Enemy Behaviour")]
    public void StartBehaviour()
    {        
        stateRoutine = StartCoroutine(UpdatePath());
    }
    // Update is called once per frame
    void Update()
    {
        if (bIsMoving)
        {
            MoveToGoal();
        }
    }

    protected abstract void SetAIState(States newState);

    protected IEnumerator UpdatePath()
    {
        bIsMoving = true;
        while (true)
        {
            Vector3 playerPos = PlayerManager.instance.player.transform.position;
            ownNode = NavOperations.GetNearestNode(transform.position, NavMesh.allNodes);
            Node playerNode = NavOperations.GetNearestNodeInPlane(playerPos, NavMesh.allNodes, ownNode.WorldPosition.y);

            ownNode.SetObstacle(true);
            if (lastNode == null)
                lastNode = ownNode;
            if(ownNode != lastNode)
            {
                lastNode.SetObstacle(false);
                lastNode = ownNode;
            }

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
