using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public int health { get; private set; }
    
    public void Damage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }

    protected abstract void CalculatePath();

    protected abstract IEnumerator Move(Vector3 target);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
