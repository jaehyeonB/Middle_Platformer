using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPot : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
