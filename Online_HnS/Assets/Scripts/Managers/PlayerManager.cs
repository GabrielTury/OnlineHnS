using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public GameObject player;

    public Vector3 playerPos, lastPos;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this);

        lastPos = playerPos;
        //player = FindFirstObjectByType<PlayerStatus>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        CheckForDiff();
    }

    private void CheckForDiff()
    {
        if (Vector3.Distance(playerPos, lastPos) > .5)
        {
            UpdatePosition(playerPos);
            lastPos = playerPos;
        }
    }
    public void UpdatePosition(Vector3 pos)
    {
        GameEvents.OnPlayerMoved(pos);
    }
}
