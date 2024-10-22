using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerAttacks/Melee Attacks")]
public class MeleePlayerAttackSO : ScriptableObject
{
    public AnimatorOverrideController animOC;
    public float damage;
    public float minTime;
}
