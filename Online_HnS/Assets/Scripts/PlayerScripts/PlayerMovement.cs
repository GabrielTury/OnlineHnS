using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController characterController;
    private PlayerControls inputActions;
    [SerializeField]
    private Animator animator;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    private bool isMovementPressed;

    public float movementSpeed;
    private Vector3 forward;
    private Vector3 right;
    private Vector3 cameraRelativeMovement;
    public float rotationFactorPerFrame = 15f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new PlayerControls();

        inputActions.Movement.Move.started += OnMovement;
        inputActions.Movement.Move.canceled += OnMovement;
        inputActions.Movement.Move.performed += OnMovement;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void OnMovement(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * movementSpeed;
        currentMovement.z = currentMovementInput.y * movementSpeed;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
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
        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    void HandleAnimatorInteraction()
    {
        animator.SetBool("isMoving", isMovementPressed);
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleCameraRelativeMovement(currentMovement);
        HandleRotation();
        HandleAnimatorInteraction();
    }
}
