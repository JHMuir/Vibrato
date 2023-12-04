using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    [Header("References")]
    private Animator anim;
    [Space]

    [Header("Player Movement Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    //[SerializeField] private float coyoteTime;
    //private float coyoteTimeCounter;
    private int airJumpCounter  = 0;
    [SerializeField] private int maxAirJumps = 1;
    [Space]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    [Space]

    //Private Variables
    private float horizontalMove = 0;
    private bool jumpInput = false;
    private bool isDashing = false;
    private bool dashInput = false;
    private bool canDash = true;
    private bool dashed;
    private float gravity;
    public bool invincible;

    public static PlayerController Instance;

    void Awake()
    {
        if(Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        health = maxHealth;
    }
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale; 
    }

    void Update()
    {
        GetInput();
        Die();
    }

    void FixedUpdate()
    {
        if(isDashing) return;
        Move(horizontalMove * Time.fixedDeltaTime);
        Jump(jumpInput);
        jumpInput = false;
        StartDash(dashInput);
    }

    void GetInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if(Input.GetButtonDown("Jump")) jumpInput = true;
        //if(Input.GetButtonUp("Jump")) jumpInput = false;
        if(Input.GetButtonDown("Dash") && canDash && !dashed)  dashInput = true;
    }

    protected override void Move(float move)
    {
        base.Move(move);
        if(!Grounded())
        {
            //If in the air, we keep control but lose some speed 
            Vector3 targetVelocity = new Vector3(move * 10f * airSpeedReduction, rb.velocity.y);
            //And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        }
        anim.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    protected override void Jump(bool jump)
    {
        if(jump && airJumpCounter < maxAirJumps)
        {
            base.Jump(jump);
            airJumpCounter++;
        }
        if(Grounded())
        {
            jumpInput = false;
            airJumpCounter = 0;
        }
        anim.SetBool("Jumping", !Grounded());
    }

    void StartDash(bool dash)
    {
        if(dash)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if(Grounded()) dashed = false;
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        if(facingRight) rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        else rb.velocity = new Vector2(-transform.localScale.x * dashSpeed, 0);
        if(Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        dashInput = false;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        anim.ResetTrigger("Dashing");
    }

    public void TakeDamage(float damage)
    {
        health -= Mathf.RoundToInt(damage);
        StartCoroutine(StopTakingDamage());
    }

    IEnumerator StopTakingDamage()
    {
        invincible = true;
        anim.SetTrigger("TakeDamage");
        //rb.AddForce(-Vector3.right * 5);
        ClampHealth();
        yield return new WaitForSeconds(1f);
        invincible = false;
    }

    void Die()
    {
       //if(health <= 0) Destroy(gameObject);
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health,0, maxHealth);
        //if(onHealthChangedCallback != null) onHealthChangedcallback.Invoke();
    }
}
