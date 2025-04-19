using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    [Header("플레이어 이동")]
    public float moveSpeed = 2.0f;
    public float jumpForce = 5.0f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float PlayerDirection = 0;

    [Header("플레이어 튕김")]
    public float BounceForce = 3f;
    public float BounceDuration = -0.5f;
    private bool isBouncedBack = false;
    private float BounceTimer = 0f;

    [Header("플레이어 HP")]
    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;
    public int MaxHealth = 3;
    public int currentHealth = 3;


    private Rigidbody2D rb;
    private Animator pAni;
    public bool isGrounded;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
    }
    private void Update()
    {

        #region 플레이어 이동
        if (!isBouncedBack)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);


            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }


            if (moveInput > 0)
            {
                transform.localScale = new Vector3(-0.15f, 0.15f, 1);
                pAni.SetBool("Run", true);
            }
            else if (moveInput < 0)
            {
                transform.localScale = new Vector3(0.15f, 0.15f, 1);
                pAni.SetBool("Run", true);
            }
            else if (moveInput == 0)
            {
                pAni.SetBool("Run", false);
            }
        }
        //넉백 플레이어 강제 이동 + 플레이어 스턴
        #endregion


        if (isBouncedBack)
        {
            BounceTimer -= Time.deltaTime;

            if (BounceTimer <= 0)
            {
                isBouncedBack = false;
            }
        }


    }
    void Start()
    {
        currentHealth = MaxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MovetoNextLevel();
        }

        if (collision.CompareTag("Tramp"))
        {
            Trampoline(collision.transform.position - transform.position);
        }
    }

    #region 너 못해
    public void Trampoline(Vector2 playerDirection)
    {
        if (!isBouncedBack)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.AddForce(-playerDirection.normalized * BounceForce, ForceMode2D.Impulse);

            isBouncedBack = true;
            BounceTimer = BounceDuration;
        }
    }
    #endregion
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            OnDamaged(collision.transform.position);
        }
    }
    void OnDamaged(Vector2 targetPos)
    {
        gameObject.layer = 11;
        currentHealth--;

        rb.velocity = Vector2.zero;

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;

        rb.velocity = new Vector2(dirc * 2f, 4f);

        isBouncedBack = true;
        BounceTimer = 0.75f;

        if(currentHealth == 3)
        {
            Heart1.SetActive(true);
            Heart2.SetActive(true);
            Heart3.SetActive(true);
        }
        else if(currentHealth == 2)
        {
            Heart1.SetActive(true);
            Heart2.SetActive(true);
            Heart3.SetActive(false);
        }
        else if(currentHealth == 1)
        {
            Heart1.SetActive(true);
            Heart2.SetActive(false);
            Heart3.SetActive(false);
        }
        else if (currentHealth == 0)
        {
            Heart1.SetActive(false);
            Heart2.SetActive(false);
            Heart3.SetActive(false);
            gameObject.SetActive(false);
            Invoke("PlayerDeath", 3f);

        }
            Invoke("OffDamaged", 3f);
    }
    void OffDamaged()
    {
        gameObject.layer = 10;
    }

    void PlayerDeath()
    {
        SceneManager.LoadScene("Title");
    }
}

    

