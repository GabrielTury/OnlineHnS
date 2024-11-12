using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    [SerializeField]
    FaceMouse _faceMouse;
    [SerializeField]
    FaceNearestDamageable _faceNearestDamageable;

    private void FixedUpdate()
    {
        if (_faceNearestDamageable.FindNearestDamageable() != null)
        {
            _faceNearestDamageable.enabled = true;
            //_faceMouse.enabled = false;
        }
        else
        {
            _faceNearestDamageable.enabled = false;
            //_faceMouse.enabled = true;
        }
    }


}
