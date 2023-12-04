using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Zombie : EnemyController
{
    // Start is called before the first frame update
    void Start()
    {
        rb.gravityScale = 12;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(float damage, Vector2 hitDirection, float hitForce)
    {
        base.TakeDamage(damage, hitDirection, hitForce);
    }

    
}
