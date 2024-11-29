using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameEvents
{
    public static event UnityAction<InputDevice> UIInputMade;
    public static void OnUIInputMade(InputDevice uiInputMade) => UIInputMade?.Invoke(uiInputMade);

    public static UnityAction Room_Start;
    public static void OnRoomStarted() => Room_Start?.Invoke();

    public static UnityAction<float, int> Player_Damaged;
    public static void OnPlayerDamaged(float damage, int playerIndex) => Player_Damaged?.Invoke(damage, playerIndex);

    public static UnityAction<int, int> Player_Skill_Used;
    public static void OnPlayerSkillUsed(int skillNum, int playerIndex) => Player_Skill_Used?.Invoke(skillNum, playerIndex);

    public static UnityAction<Transform> Damageable_Spawn;
    public static void OnDamageableSpawned(Transform objectTransform) => Damageable_Spawn?.Invoke(objectTransform);

    public static UnityAction<NetworkObject> Player_Spawn;
    public static void OnPlayerSpawn(NetworkObject playerObj) => Player_Spawn?.Invoke(playerObj);

}
