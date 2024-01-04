using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Callbacks;
using UnityEditor.Tilemaps;
using UnityEditor.Timeline;
using UnityEngine;

public class Zombie : EnemyController
{

    [SerializeField] private Transform rushPoint;
    [Range(0, 10f)] [SerializeField] protected float aggroRange;
    [Range(0, 100f)] [SerializeField] protected float rushForce;
    private bool move = true;
    //[SerializeField] private LineRenderer lineRender;
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
        float x = transform.position.x;
        ZombieMove(move);
        //transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.fixedDeltaTime);
        if(x < transform.position.x && !facingRight) Flip();
        else if(x > transform.position.x && facingRight) Flip();
        if(RushCast(aggroRange)) StartCoroutine(Rush());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
    }
    void ZombieMove(bool move)
    {
        if(move) transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.fixedDeltaTime);
    }

    public override void TakeDamage(float damage, Vector2 hitDirection, float hitForce)
    {
        base.TakeDamage(damage, hitDirection, hitForce);
    }

    bool RushCast(float distance)
    {
        bool val = false;
        var direction = 1;
        if(!facingRight) direction = -1;
        Vector2 endPos = rushPoint.position + Vector3.right * distance * direction;
        RaycastHit2D hit = Physics2D.Linecast(rushPoint.position, endPos, 1 << LayerMask.NameToLayer("Interactable"));
        Debug.DrawLine(rushPoint.position, endPos, Color.blue);

        if(hit.collider != null)
        {
            if(hit.collider.gameObject.CompareTag("Player")) val = true;
            else val = false;
            Debug.DrawLine(rushPoint.position, hit.point, Color.red);
            
        }
        else Debug.DrawLine(rushPoint.position, endPos, Color.blue);
        return val;
    }
    IEnumerator Rush()
    {
        /*Function that runs once the raycast has hit the player. Zombie dashes foward, then resumes moving. */
        float currAggro = aggroRange;
        //Vector2 currSpeed = rb.velocity;
        aggroRange = 0;
        if(facingRight) rb.velocity = new Vector2(transform.localScale.x * rushForce, 0);
        else rb.velocity = new Vector2(-transform.localScale.x * rushForce, 0);
        move = false;
        yield return new WaitForSeconds(2);
        //rb.velocity = Vector2.zero;
        aggroRange = currAggro;
        move = true;
        //rb.velocity = currSpeed; 
        
    }

    protected override void Attack(Vector2 playerRecoilDirection)
    {
        base.Attack(playerRecoilDirection);
        StartCoroutine(StopMoving());
    }

    IEnumerator StopMoving()
    {
        move = false;
        yield return new WaitForSeconds(2);
        move = true;
    }
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    
}
