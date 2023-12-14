using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountCoroutineManager : MonoBehaviour
{
    public static CountCoroutineManager Instance;

    private void Awake()
    {
        //singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }


    public Coroutine StartManagedCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void StopManagedCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
}
