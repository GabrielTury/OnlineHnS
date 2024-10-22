using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public int health { get; private set; }

    protected Coroutine moveRoutine;
    
    public void Damage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Death();
        }
    }
    protected void Death()
    {
        Destroy(gameObject);
    }

    protected abstract void CalculatePath(Vector3 pos);    

    protected abstract IEnumerator Move(Vector3 target);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GameEvents.Player_Move += CalculatePath;
    }

    private void OnDisable()
    {
        GameEvents.Player_Move -= CalculatePath;
    }
}
