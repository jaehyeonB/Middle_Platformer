using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Null : MonoBehaviour
{

    public float moveSpeed = 2.0f;
    public float raycastDistance = .2f;
    public float traceDistance = 2f;

    private Transform player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector2 direction = player.position - transform.position;

        if(direction.magnitude > traceDistance)
        return;

        Vector2 directionNormalized = direction.normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionNormalized, raycastDistance);
        Debug.DrawRay(transform.position, directionNormalized * raycastDistance, Color.red);

        foreach(RaycastHit2D rHit in hits)
        {
            if(rHit.collider != null && rHit.collider.CompareTag("Obstacle"))
            {
                Vector3 alternativeDirection = Quaternion.Euler(0f, 0f, -90f) * direction;
                transform.Translate(alternativeDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }

    }
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            
        }
    }*/
}
