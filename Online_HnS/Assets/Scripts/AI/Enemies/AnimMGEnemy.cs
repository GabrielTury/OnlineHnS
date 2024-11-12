using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimMGEnemy : MonoBehaviour
{
    private GroundEnemy behaviour;

    private void Start()
    {
        behaviour = GetComponentInParent<GroundEnemy>();
    }
    public void AttackEventCall()
    {
        behaviour.AttackBehaviour();
    }

    public void AttackEndCall()
    {
        behaviour.AttackEnd();
    }
    public void HitEventCall()
    {

    }
}
