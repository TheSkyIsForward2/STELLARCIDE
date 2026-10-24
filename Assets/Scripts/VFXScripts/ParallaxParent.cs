using System.Collections.Generic;
using UnityEngine;

public class ParallaxParent : MonoBehaviour
{
    [SerializeField] private float zScale = 10f;
    GameObject player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, zScale);
    }
    
    
}
