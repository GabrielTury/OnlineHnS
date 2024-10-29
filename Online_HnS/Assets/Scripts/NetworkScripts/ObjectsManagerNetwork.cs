using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectsManagerNetwork : NetworkBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    AudioListener audioListener;
    [SerializeField]
    RotationManager rotationManager;
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            audioListener.enabled = true;
            rotationManager.enabled = true;
            virtualCamera.Priority = 1;
        }
        else 
        {
            virtualCamera.Priority = 0;
            rotationManager.enabled = false;
        }

    }
}
