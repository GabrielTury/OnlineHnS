using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController characterController;
    private PlayerControls inputActions;
    private UIControls uiActions;
    [SerializeField]
    private Animator anim;
    

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    private bool isMovementPressed;
    public bool canMove = true;
    public bool canRotate = true;

    public float movementSpeed;
    private Vector3 forward;
    private Vector3 right;
    private Vector3 cameraRelativeMovement;
    public float rotationFactorPerFrame = 15f;
    public float dashTime;
    public float dashSpeed;

    private Transform parent;

    [SerializeField] private GameObject pauseCanvas;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new PlayerControls();
        uiActions = new UIControls();

        inputActions.Movement.Move.started += OnMovement;
        inputActions.Movement.Move.canceled += OnMovement;
        inputActions.Movement.Move.performed += OnMovement;

        inputActions.Movement.Dash.started += OnDash;
        inputActions.Movement.Dash.performed += OnDash;

        parent = transform.parent;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        uiActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
        uiActions.Disable();
    }

    void OnMovement(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * movementSpeed;
        currentMovement.z = currentMovementInput.y * movementSpeed;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void OnDash(InputAction.CallbackContext context)
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("LightAttack"))
        {
            anim.SetTrigger("dash");
            StartCoroutine(Dash());
        }
        
    }


    void HandleCameraRelativeMovement(Vector3 moveVector)
    {
        forward = Camera.main.transform.forward;
        right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeMovement = moveVector.z * forward;
        Vector3 rightRelativeMovement = moveVector.x * right;
        cameraRelativeMovement = forwardRelativeMovement + rightRelativeMovement;

        if (characterController != null)
        {
            characterController.Move(cameraRelativeMovement * Time.deltaTime);
            
        }

    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = cameraRelativeMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = cameraRelativeMovement.z;
        Quaternion currentRotation = transform.rotation;
        if (isMovementPressed && canRotate)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    void HandleAnimatorInteraction()
    {
        anim.SetBool("isMoving", isMovementPressed);
    }

    private void Update()
    {
        if (!IsOwner) return;

        parent.position = transform.position;
        if (canMove)
        {
            HandleCameraRelativeMovement(currentMovement);
            HandleRotation();
        }
        else
        {
            HandleCameraRelativeMovement(Vector3.zero);
            //isMovementPressed = false;
        }
        transform.position = new Vector3(transform.position.x, 1.58f, transform.position.z);
        HandleAnimatorInteraction();

        if (Time.timeScale == 1f && uiActions.UI.Pause.WasPressedThisFrame())
        {
            Instantiate(pauseCanvas);
        }
    }


    public void ChangeAnimator(Animator animator)
    {
        anim = animator;
    }
    IEnumerator Dash()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
