using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : NetworkBehaviour
{
    public static NetworkUIManager instance;

    [SerializeField]
    private Button startServerButton;
    [SerializeField]
    private Button startHostButton;
    [SerializeField]
    private Button startClientButton;
    [SerializeField]
    private TextMeshProUGUI playersInGameText;
    public TMP_Text idText;

    private NetworkVariable<int> playerID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        Cursor.visible = true;
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startHostButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartHost())
            {
                print("Host started...");
                StartCoroutine(CheckPlayers());
            }
            else
            {
                print("Host could not be started...");
            }
        });

        //startServerButton.onClick.AddListener(() =>
        //{
        //    if (NetworkManager.Singleton.StartServer())
        //    {
        //        print("Server started...");
        //    }
        //    else
        //    {
        //        print("Server could not be started...");
        //    }
        //});

        startClientButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartClient())
            {
                print("Client started...");
                StartCoroutine(CheckPlayers());
            }
            else
            {
                print("Client could not be started...");
            }
        });
    }

    public void GetID()
    {
        idText.text = string.Format("Client ID: {0}", playerID.Value);
    }


    private void Update()
    {
        //playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    private IEnumerator CheckPlayers()
    {
        while (IsServer)
        {
            GetID();
            playerID.Value = NetworkManager.Singleton.ConnectedClients.Count;
            yield return new WaitForSeconds(0.25f);
        }

        while (!IsServer)
        {
            GetID();
            yield return new WaitForSeconds(0.25f);
        }
        yield return null;
    }
}
