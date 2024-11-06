using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Tooltip("List of light melee attacks")]
    public List<MeleePlayerAttackSO> lightMeleeCombo;

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private Transform detectionCenter;
    [SerializeField]
    private LayerMask layerMask;
    private PlayerControls inputActions;
    private float lastClickedTime;
    private float lastComboEnd;
    public int comboCounter;
    private float comboCooldownTimer = 0.0f;
    private float maxComboDelay = 0.8f;

    private bool isAttacking = false;


    private void Awake()
    {
        inputActions = new PlayerControls();

        inputActions.Attack.LightAttack.started += OnLightAttack;
        inputActions.Attack.LightAttack.canceled += OnLightAttack;

        inputActions.Attack.HeavyAttack.started += OnHeavyAttack;
        inputActions.Attack.HeavyAttack.canceled += OnHeavyAttack;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnLightAttack(InputAction.CallbackContext context)
    {
        isAttacking = context.ReadValueAsButton();
        if (isAttacking)
        {
            Attack();
            
        }
    }
    private void OnHeavyAttack(InputAction.CallbackContext context)
    {

    }
    private void DetectDamageables()
    {
        Collider[] damageableColliders = Physics.OverlapSphere(detectionCenter.position, lightMeleeCombo[comboCounter].radius, layerMask);
        foreach (Collider collider in damageableColliders)
        {
            var objectInterface = collider.GetComponent<IDamageable>();
            if(objectInterface != null)
            {
                //objectInterface.Damage(lightMeleeCombo[comboCounter].damage);
                print(collider.name);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(detectionCenter.position, lightMeleeCombo[comboCounter].radius);
    }
    private void Update()
    {
        ExitAttack();
    }
    private void Attack()
    {
        if (Time.time - lastComboEnd > 0.5f && comboCounter < lightMeleeCombo.Count)
        {
            CancelInvoke("EndCombo");
            // Start the combo if it's the first attack or we're still within the allowed delay
            if (Time.time - lastClickedTime >= lightMeleeCombo[comboCounter].minTime)
            {
                anim.runtimeAnimatorController = lightMeleeCombo[comboCounter].animOC;
                anim.Play("LightAttack", 0, 0);
                comboCounter++;
                lastClickedTime = Time.time;
                DetectDamageables();

                // Reset combo at the end of the sequence
                if (comboCounter >= lightMeleeCombo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
        if (comboCounter >= lightMeleeCombo.Count)
        {
            comboCounter = 0;
        }
    }

    private void ExitAttack()
    {
        // Check if the current attack animation is near the end
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("LightAttack"))
        {
            Invoke("EndCombo", 1f);
        }
    }

    private void ResetComboCounter()
    {
        if (comboCounter >= lightMeleeCombo.Count)
        {
            comboCounter = 0;
        }
    }

    private void EndCombo()
    {
        print("EndCombo");
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
}
