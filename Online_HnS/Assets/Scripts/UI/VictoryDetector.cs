using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryDetector : MonoBehaviour
{
    [SerializeField] private GameObject victory;

    private void OnTriggerEnter(Collider other)
    {
        victory.SetActive(true);
        Time.timeScale = 0f;
    }
}
