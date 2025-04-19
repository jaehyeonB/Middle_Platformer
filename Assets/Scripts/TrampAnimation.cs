using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampAnimation : MonoBehaviour
{
    private Animator TramAni;

    private void Awake()
    {
        TramAni = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            TramAni.SetTrigger("TrampUsed");
        }
    }
}
