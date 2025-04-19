using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    private float moveSpeed = 2.0f;
    private Rigidbody2D rb;
    private Animator pAni;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if(Input.GetAxisRaw("Horizontal") > 0)
        {
            pAni.SetBool("Run", true);
        }
        else if(Input.GetAxisRaw("Horizontal") < 0)
        {
            pAni.SetBool("Run", true);
        }
        else if(Input.GetAxisRaw("Horizontal") == 0)
        {
            pAni.SetBool("Run", false);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            pAni.SetTrigger("Jump");
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
