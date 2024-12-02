using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class EnemyBase : NetworkBehaviour, IDamageable
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
    [SerializeField]
    protected States currentState;
    #endregion
    [SerializeField]
    protected int health;

    public float moveSpeed { get; protected set; } = 5f;

    public float rotationSpeed { get; protected set; } = 4;

    protected Coroutine stateRoutine;

    protected Coroutine moveRoutine;
    protected bool bIsMoving;

    protected Node ownNode;
    protected Node lastNode;

    protected Node lastPlayerNode;

    protected Node[] chunk;

    protected Transform closestPlayer;

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
        else if(currentState != States.Hit)
        {
            SetAIState(States.Hit);            
        }
    }
    protected IEnumerator Death()
    {
        SetAIState(States.Death);
        anim.SetTrigger("Death");
        GameEvents.OnEnemyKilled(gameObject);
        yield return new WaitForSeconds(3);
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
        if(chunk == null)
            chunk = NavOperations.CreateChunk(transform.position, NavMesh.allNodes);
    }
    [Button("Start Enemy Behaviour")]
    public void StartBehaviour()
    {
        foreach (NetworkObject n in PlayerManager.instance.players)
        {
            if (closestPlayer == null)
            {
                closestPlayer = n.GetComponentInChildren<Transform>();
            }
            else if (Vector3.Distance(transform.position, n.transform.position) < Vector3.Distance(transform.position, closestPlayer.transform.position))
            {
                closestPlayer = n.GetComponentInChildren<Transform>();
            }
        }

        SetAIState(States.Moving);
    }
    // Update is called once per frame
    void Update()
    {
        if (bIsMoving)
        {
            MoveToGoal();
        }
        if(PlayerManager.instance.players.Count != 0)
        {
            //Updates closest player
            foreach(NetworkObject n in PlayerManager.instance.players)
            {
                if(closestPlayer == null)
                {
                    closestPlayer = n.GetComponentInChildren<Transform>();
                }
                else if (Vector3.Distance(transform.position, n.transform.position) < Vector3.Distance(transform.position, closestPlayer.transform.position))
                {
                    closestPlayer = n.GetComponentInChildren<Transform>();
                }
            }
        }
    }

    protected abstract void SetAIState(States newState);

    protected IEnumerator UpdatePath()
    {
        bIsMoving = true;
        if (chunk == null)
            chunk = NavOperations.CreateChunk(transform.position, NavMesh.allNodes);
        while (true)
        {
            Vector3 playerPos = closestPlayer.position;
            ownNode = NavOperations.GetNearestNode(transform.position, chunk);
            Node playerNode = NavOperations.GetNearestNodeInPlane(playerPos, chunk, ownNode.WorldPosition.y);

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
