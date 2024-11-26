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
    private ulong _PlayerId;

    private GameObject hosts_Jeriff;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;



        GameEvents.OnPlayerSpawn(GetComponent<NetworkObject>());


        _PlayerId = OwnerClientId;
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
            jakePrefab.SetActive(false);
            jeriffPrefab.SetActive(true);

        }
    }

    [ClientRpc]
    private void SetPrefabClientRpc()
    {
        while (jeriffPrefab.activeSelf)
            jeriffPrefab.SetActive(false);

        jakePrefab.SetActive(true);
        Debug.Log("ServerConnected");

    }
    [ServerRpc]
    private void SetPrefabclientServerRpc()
    {
        SpreadMessageClientRpc();
        while (NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.GetComponent<PrefabSelector>().jeriffPrefab.activeSelf)
            NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.GetComponent<PrefabSelector>().jeriffPrefab.SetActive(false);

    }
    [ClientRpc]
    private void SpreadMessageClientRpc()
    {
        jakePrefab.SetActive(false);
        jeriffPrefab.SetActive(true);
        Debug.Log("ClientConnected");

    }
}