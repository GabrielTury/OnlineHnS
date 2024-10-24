using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this);
        //player = FindFirstObjectByType<PlayerStatus>().gameObject;
    }


}
