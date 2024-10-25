using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DummyPlayer : MonoBehaviour
{
    private PlayerControls input;

    Vector3 target;

    private void Awake()
    {
        input = new PlayerControls();
    }
    private void OnEnable()
    {
        input.Enable();
        input.Movement.Move.performed += Move;
        input.Movement.Move.canceled += Move;
    }
    private void Move(InputAction.CallbackContext ctx)
    {
        Vector2 moveInput = ctx.ReadValue<Vector2>();
        target = new Vector3(moveInput.x, 0, moveInput.y);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += target * Time.deltaTime * 4;
    }
}
