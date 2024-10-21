using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputUtils : MonoBehaviour
{
    private UIControls inputActions;
    //public static InputDevice lastInputDevice;

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Awake()
    {
        inputActions = new UIControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onActionChange += ReportDevice;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Everytime an UI button is pressed, saves the last input device
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="change"></param>
    public void ReportDevice(object obj, InputActionChange change)
    {
        if (obj != null && obj is InputAction action)
        { 
            if (action.activeControl == null) return;
            InputDevice lastDevice = action.activeControl.device;

            //print(lastDevice.name);
            //lastInputDevice = lastDevice;
            GameEvents.OnUIInputMade(lastDevice);
        }
    }
}
