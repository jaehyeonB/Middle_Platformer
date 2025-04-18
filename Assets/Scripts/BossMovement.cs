using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public int BossHealth = 30;
    public float BossAttackSpeed = 1;
    public float moveSpeed = 1.5f;

    
    

    void Start()
    {
        
    }

    void Update()
    {
        if(BossHealth <= 0)
        {
            //GameObject.Destroy(string = "Boss");

        }
    }
}
