using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    [Header("기본 설정")]
    public float maxHealth = 1000;             // 보스 최대 체력
    private float currentHealth;               // 현재 체력

    public float moveSpeed = 1f;             // 이동 속도
    public float attackRange = 1f;             // 공격 사거리
    public int meleeDamage = 30;               // 기본 근접 공격 데미지
    public float attackCooldown = 2f;          // 공격 쿨타임

    private float lastAttackTime = 0f;         // 마지막 공격 시간 기록
    private bool isInvincible = false;         // 무적 상태 여부
    private bool isEnraged = false;            // 분노 상태 여부

    [Header("스킬 관련")]
    public GameObject projectilePrefab;        // 스킬2: 투사체 프리팹

    [Header("참조")]
    public Transform player;                   // 플레이어 위치
    public LayerMask playerLayer;              // 플레이어 감지 레이어
    private Animator animator;                 // 애니메이터 참조

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        // 무적 상태이거나 플레이어가 없으면 아무것도 하지 않음
        if (player == null || isInvincible) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // 공격 쿨타임이 지났을 때만 공격 실행
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;

                if (!isEnraged)
                {
                    MeleeAttack();             // 일반 근접 공격
                }
                else
                {
                    UseRandomSkill();          // 스킬 사용
                }
            }
        }
        else
        {
            MoveTowardsPlayer();               // 사거리 밖이면 이동
        }
    }
    // 플레이어를 향해 천천히 이동
    void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        animator.SetBool("isMoving", true); // 걷기 애니메이션
    }
    

    // 기본 근접 공격
    void MeleeAttack()
    {
        animator.SetTrigger("Attack");         // 애니메이션 트리거
        DealDamageInRange();                   // 근접 데미지 처리

        // 체력이 절반 이하가 되면 분노 상태 진입
        if (!isEnraged && currentHealth <= maxHealth / 2)
        {
            StartCoroutine(EnragePhase());
        }
    }

    // 근처 플레이어에게 데미지 입힘
    void DealDamageInRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            hit.GetComponent<PlayerMovement>()?.OnDamaged(transform.position);
        }
    }
    // 체력 절반 이하일 때 실행되는 분노 모드
    IEnumerator EnragePhase()
    {
        isInvincible = true;
        animator.SetTrigger("Enrage");         // 분노 애니메이션
        yield return new WaitForSeconds(3f);   // 무적 + 연출 시간

        isEnraged = true;
        isInvincible = false;
    }

    // 3가지 스킬 중 하나를 랜덤으로 선택
    void UseRandomSkill()
    {
        int skill = Random.Range(0, 3);
        switch (skill)
        {
            case 0:
                StartCoroutine(Skill_DoubleSlash());      // 이중 베기
                break;
            case 1:
                StartCoroutine(Skill_ProjectileAndDash()); // 투사체 → 돌진
                break;
            case 2:
                StartCoroutine(Skill_BlinkAndStun());      // 순간이동 + 경직
                break;
        }
    }
    // ▶ 스킬 1: 이중 베기
    IEnumerator Skill_DoubleSlash()
    {
        animator.SetTrigger("DoubleSlash");
        yield return new WaitForSeconds(0.3f);     // 첫 번째 베기
        DealDamageInRange();
        yield return new WaitForSeconds(0.4f);     // 두 번째 베기
        DealDamageInRange();
    }

    // ▶ 스킬 2: 투사체 발사 후 돌진
    IEnumerator Skill_ProjectileAndDash()
    {
        animator.SetTrigger("Throw");

        // 투사체를 플레이어 방향으로 발사
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Rigidbody2D>().velocity = direction * 5f;

        yield return new WaitForSeconds(1f);       // 1초 후 투사체 위치 기억

        Vector3 dashTarget = proj != null ? proj.transform.position : transform.position;
        if (proj != null) Destroy(proj);           // 투사체 제거

        animator.SetTrigger("Dash");

        // 돌진 연출 (짧은 시간 동안 보스 이동)
        float dashTime = 0.4f;
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < dashTime)
        {
            transform.position = Vector3.Lerp(start, dashTarget, elapsed / dashTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = dashTarget;
        DealDamageInRange();
    }

    // ▶ 스킬 3: 순간이동 후 경직
    IEnumerator Skill_BlinkAndStun()
    {
        animator.SetTrigger("Blink");

        // 플레이어 왼쪽으로 순간이동
        Vector3 blinkPosition = player.position + Vector3.left * 1.5f;
        transform.position = blinkPosition;

        isInvincible = true;
        animator.SetBool("isStunned", true);    // 경직 상태 표시

        yield return new WaitForSeconds(0.75f); // 경직 시간

        animator.SetBool("isStunned", false);
        isInvincible = false;
    }
    // 외부에서 데미지를 받는 함수
    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            animator.SetTrigger("Die");         // 사망 애니메이션
            Destroy(gameObject, 2f);            // 잠시 후 제거
        }
    }

    // 씬에서 공격 범위를 시각적으로 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

