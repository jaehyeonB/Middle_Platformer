using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    
    [Header("플레이어 넉백")]
    public float knockbackForce = 10.0f;             //넉백 힘
    public float knockbackDuration = -1.5f;         //넉백 지속 시간
    private bool isKnockedBack = false;             //넉백이 되었는가
    private float KnockBackTimer = 0f;              //넉백 타이머

    [Header("플레이어 HP")]
    public int PlayerHealth = 3;

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
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if(isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(-0.15f, 0.15f, 1);
            pAni.SetBool("Run",true);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(0.15f, 0.15f, 1);
            pAni.SetBool("Run",true);
        }
        else if (moveInput == 0)
            pAni.SetBool("Run",false);
        //넉백 플레이어 강제 이동 + 플레이어 스턴
        #endregion


        if (isKnockedBack)
        {
            KnockBackTimer -= Time.deltaTime;

            if(KnockBackTimer <= 0)
            {
                isKnockedBack = false;
            }
        }
    }
    void Start()
    {
        
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

        if ( collision.CompareTag("Tramp"))         
        {
            PlayerHealth --;
            TakeDamage(collision.transform.position - transform.position);
        }
    }

    #region 너 못해
    public void TakeDamage(Vector2 playerDirection)
    {
        if (!isKnockedBack)
        {
            // 넉백 효과 적용
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero; // 현재 속도를 초기화
            rb.AddForce(-playerDirection.normalized * knockbackForce, ForceMode2D.Impulse); // 넉백 방향으로 힘을 가함

 
            // 넉백 상태 설정 및 타이머 초기화
            isKnockedBack = true;
            KnockBackTimer = knockbackDuration;
        }
    }
    #endregion
}
