using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameEvents
{
    public static event UnityAction<InputDevice> UIInputMade;
    public static void OnUIInputMade(InputDevice uiInputMade) => UIInputMade?.Invoke(uiInputMade);

    public static UnityAction Room_Start;
    public static void OnRoomStarted() => Room_Start?.Invoke();

    public static UnityAction<Transform> Damageable_Spawn;
    public static void OnDamageableSpawned(Transform objectTransform) => Damageable_Spawn?.Invoke(objectTransform);

}
