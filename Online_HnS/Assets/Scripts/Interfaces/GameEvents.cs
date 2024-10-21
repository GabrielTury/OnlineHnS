using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameEvents
{
    public static event UnityAction<InputDevice> UIInputMade;
    public static void OnUIInputMade(InputDevice uiInputMade) => UIInputMade?.Invoke(uiInputMade);
}
