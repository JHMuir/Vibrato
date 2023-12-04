using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehavior : MonoBehaviour
{
    public float speed = 10.0f;
    public float damage = 40f;
    public float liveTime = 2f;
    public GameObject impactEffect;
    public Rigidbody2D rb;

    void Start()
    {
        //rb.velocity = transform.right * speed;
        Destroy(gameObject, liveTime);
    }

    void FixedUpdate()
    {
        rb.velocity = transform.right * speed;//* Time.fixedDeltaTime;
    }

    void OnTriggerEnter2D(Collider2D noteHit)
    {
       if(noteHit.gameObject.CompareTag("Enemy")) 
       {
            EnemyController enemy = noteHit.GetComponent<EnemyController>();
            if(enemy != null) enemy.TakeDamage(damage, (transform.position - enemy.transform.position).normalized, 100);
       }
       else if(noteHit.gameObject.CompareTag("Player"))
       {
            //do nothing
       }
       Instantiate(impactEffect, transform.position, transform.rotation);
       Destroy(gameObject);
    }
}
