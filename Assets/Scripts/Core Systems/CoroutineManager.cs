using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Run(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}