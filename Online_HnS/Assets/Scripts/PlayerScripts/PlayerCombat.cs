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
    private PlayerControls inputActions;
    private float lastClickedTime;
    private float lastComboEnd;
    private int comboCounter;
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

    private void Update()
    {
        ExitAttack();

        if (comboCooldownTimer > 0)
        {
            comboCooldownTimer -= Time.deltaTime;
            if (comboCooldownTimer <= 0)
            {
                EndCombo();
            }
        }
    }
    private void Attack()
    {
        if (Time.time - lastComboEnd > lightMeleeCombo[comboCounter].minTime && comboCounter < lightMeleeCombo.Count)
        {
            // Start the combo if it's the first attack or we're still within the allowed delay
            if (Time.time - lastClickedTime >= lightMeleeCombo[comboCounter].minTime)
            {
                anim.runtimeAnimatorController = lightMeleeCombo[comboCounter].animOC;
                anim.Play("LightAttack", 0, 0);
                comboCounter++;
                lastClickedTime = Time.time;

                // Reset cooldown timer to max delay time for each successful attack
                comboCooldownTimer = maxComboDelay;

                // Reset combo at the end of the sequence
                if (comboCounter >= lightMeleeCombo.Count)
                {
                    comboCounter = 0;
                    comboCooldownTimer = 0.1f; // Short cooldown after last combo hit
                }
            }
        }
    }

    private void ExitAttack()
    {
        // Check if the current attack animation is near the end
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("LightAttack"))
        {
            // Start the cooldown timer only if we're finishing the last hit or in an idle period
            if (comboCounter >= lightMeleeCombo.Count && comboCooldownTimer <= 0)
            {
                comboCooldownTimer = 0.1f;
            }
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
        comboCooldownTimer = 0;
    }
}
