using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PrefabSelector : NetworkBehaviour
{
    [SerializeField] GameObject jakePrefab;
    [SerializeField] GameObject jeriffPrefab;
    [SerializeField] PlayerMovement _PlayerMovement;
    [SerializeField] PlayerCombat _PlayerCombat;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        if (IsHost)
        {
            SetPrefabClientRpc();
            _PlayerMovement.ChangeAnimator(jakePrefab.GetComponent<Animator>());
            _PlayerCombat.ChangeAnimator(jakePrefab.GetComponent<Animator>());
            _PlayerCombat.isMelee = true;
        }
        else if (IsClient)
        {
            SetPrefabclientServerRpc();
            _PlayerMovement.ChangeAnimator(jeriffPrefab.GetComponent<Animator>());
            _PlayerCombat.ChangeAnimator(jeriffPrefab.GetComponent<Animator>());
        }
    }

    [ClientRpc]
    private void SetPrefabClientRpc()
    {
        jeriffPrefab.SetActive(false);
        jakePrefab.SetActive(true);
        Debug.Log("ServerConnected");

    }
    [ServerRpc]
    private void SetPrefabclientServerRpc()
    {
        SpreadMessageClientRpc();
    }
    [ClientRpc]
    private void SpreadMessageClientRpc()
    {
        jakePrefab.SetActive(false);
        jeriffPrefab.SetActive(true);
        Debug.Log("ClientConnected");
    }
}
