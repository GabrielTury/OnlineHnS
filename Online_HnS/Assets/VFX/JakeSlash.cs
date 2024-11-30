using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JakeSlash : MonoBehaviour
{

    public Animator anim;
    public List<Slash> slashes;
    void Start()
    {
        DisableSlashes();
        
    }

    public void BeginVFXSequence(int comboCount)
    {
        StartCoroutine(SlashAttack(comboCount));
    }
    IEnumerator SlashAttack(int slashToHandle)
    {
       
        yield return new WaitForSeconds(slashes[slashToHandle].delay);
        slashes[slashToHandle].slashObj.SetActive(true);

        yield return new WaitForSeconds(1);
        DisableSlashes();
    }

     void DisableSlashes()
    {
        for(int i = 0;i < slashes.Count;i++)
            slashes[i].slashObj.SetActive(false);
    }
}

[System.Serializable]

public class Slash
{

    public GameObject slashObj;
    public float delay;
}