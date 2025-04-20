using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacked : MonoBehaviour
{
    [Header("공통 체력 시스템")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isInvincible = false;
    public bool isBoss = false;
    public bool isPhaseTwo = false;

    public GameObject Speedpotion;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (GetComponent<BossMove>() != null)
        {
            isBoss = true;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;

        if (isBoss && !isPhaseTwo && currentHealth <= maxHealth / 2)
        {
            StartCoroutine(EnterPhaseTwo());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator EnterPhaseTwo()
    {
        isInvincible = true;
        if (animator != null)
        {
            animator.SetTrigger("PhaseChange");
        }
        yield return new WaitForSeconds(2f);

        isPhaseTwo = true;
        isInvincible = false;
    }

    void Die()
    {
        if (isBoss)
        {
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            GetComponent<BossMove>().enabled = false;
        }
        else
        {
            Speedpotion.SetActive(true);
            Speedpotion.transform.position = transform.position;
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
}
