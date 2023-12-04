using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    
    protected bool isRecoiling = false;
    protected float recoilTimer;
    protected  Rigidbody2D rb;
    protected bool facingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(health <= 0)Destroy(gameObject);
        if(isRecoiling)
        {
            if(recoilTimer < recoilLength) recoilTimer += Time.deltaTime;
            else
            {
                isRecoiling = false; 
                recoilTimer = 0;
            }
        }
    }

    public virtual void TakeDamage(float damage, Vector2 hitDirection, float hitForce)
    {
        health -= damage;  
        if(!isRecoiling) {
            rb.AddForce(-hitForce * recoilFactor * hitDirection);
            isRecoiling = true;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D enemyHit)
    {
        if(enemyHit.CompareTag("Player") && !PlayerController.Instance.invincible) Attack();
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}

/* private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
//If the input is moving the character right and facing left
        if(move > 0 && !facingRight) Flip();
        //otherwise, character is moving left and facing right
        else if(move < 0 && facingRight) Flip();*/
