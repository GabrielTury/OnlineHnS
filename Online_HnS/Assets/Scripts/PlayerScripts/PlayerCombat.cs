using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : NetworkBehaviour
{
    [Tooltip("List of light melee attacks")]
    public List<MeleePlayerAttackSO> lightMeleeCombo;

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private FaceMouse faceMouse;
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private Transform detectionCenter;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private LayerMask layerMask;
    private PlayerControls inputActions;
    private float lastClickedTime;
    private float lastComboEnd;
    public int comboCounter;
    public bool isMelee = false;

    private bool isDead = false;
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
        if (isAttacking && IsOwner && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Damage"))
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

    public void ChangeAnimator(Animator animator)
    {
        anim = animator;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(detectionCenter.position, lightMeleeCombo[comboCounter].radius);
    }
    private void Update()
    {
        if(isDead)
        { return; }

        ExitAttack();
        ExitDamage();
    }
    private void Attack()
    {
        if(isMelee)
        {
            if (Time.time - lastComboEnd > 0.5f && comboCounter < lightMeleeCombo.Count)
            {
                CancelInvoke("EndCombo");
                // Start the combo if it's the first attack or we're still within the allowed delay
                if (Time.time - lastClickedTime >= lightMeleeCombo[comboCounter].minTime)
                {
                    playerMovement.canRotate = false;
                    faceMouse.FaceMouse3D();
                    anim.runtimeAnimatorController = lightMeleeCombo[comboCounter].animOC;
                    anim.Play("LightAttack", 0, 0);

                    if (comboCounter < lightMeleeCombo.Count)
                    {

                        comboCounter++;
                    }
                    lastClickedTime = Time.time;
                    //DetectDamageables();

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
        else
        {
            if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("LightAttack"))
            {
                anim.Play("Idle", 0, 0);
                playerMovement.canMove = false;
                faceMouse.FaceMouse3D();
                anim.SetTrigger("attack");
                HandleShoot();
            }
            
        }

        
    }

    private void ExitAttack()
    {
        // Check if the current attack animation is near the end
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("LightAttack"))
        {
            playerMovement.canMove = true;
            playerMovement.canRotate = true;
            Invoke("EndCombo", 1f);
        }
        else if(isMelee && !anim.GetCurrentAnimatorStateInfo(0).IsTag("LightAttack"))
        {
            playerMovement.canMove = true;
            playerMovement.canRotate = true;
        }
    }

    private void ExitDamage()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("Damage"))
        {
            playerMovement.canMove = true;
            playerMovement.canRotate = true;
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

    public void HandleDamage()
    {
        anim.Play("Idle", 0, 0);
        playerMovement.canMove = false;
        anim.SetTrigger("damage");
    }

    public void HandleDeath()
    {
        anim.Play("Idle", 0, 0);
        playerMovement.canMove = false;
        isDead = true;
        anim.SetTrigger("death");
    }
    private void HandleShoot()
    {
        if (!IsOwner) return;

        ShootBulletServerRpc();
    }
    [ServerRpc]
    private void ShootBulletServerRpc()
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, detectionCenter.position, detectionCenter.rotation);
        NetworkObject networkObject = bulletInstance.GetComponent<NetworkObject>();

        //if (networkObject != null)
        {
            bulletInstance.GetComponent<Bullet>().playerGameObject = gameObject;
            networkObject.Spawn();  // Spawn bullet across the network
        }
    }
}
