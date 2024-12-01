using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimFREnemy : MonoBehaviour
{
    private FlyingEnemy eScript;

    private void Awake()
    {
        eScript = GetComponentInParent<FlyingEnemy>();
    }
    public void AttackTiming()
    {
        eScript.ShootBulet();
    }
}
