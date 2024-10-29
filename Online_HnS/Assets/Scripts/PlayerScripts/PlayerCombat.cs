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
    }
    private void Attack()
    {
        if(Time.time - lastComboEnd > lightMeleeCombo[comboCounter].minTime && comboCounter <= lightMeleeCombo.Count)
        {
            CancelInvoke("EndCombo");
            if (Time.time - lastClickedTime >= lightMeleeCombo[comboCounter].minTime)
            {
                anim.runtimeAnimatorController = lightMeleeCombo[comboCounter].animOC;
                anim.Play("LightAttack", 0, 0);
                comboCounter++;
                lastClickedTime = Time.time;
                ResetComboCounter();
            }
        }
    }

    private void ExitAttack()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("LightAttack"))
        {
            Invoke("EndCombo", 1);
        }
        ResetComboCounter();
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
        comboCounter = 0;
        lastComboEnd = Time.time;        
    }
}
