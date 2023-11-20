using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Engine based stats
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    [Space(3)]

    [Header("Dash Settings")]
    private bool canDash = true;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    private bool dashed;
    [Space(3)]

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 20; 
    [SerializeField] private int jumpBufferFrames;
    private int jumpBufferCounter = 0;
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;
    private int airJumpCounter  = 0;
    [SerializeField] private int maxAirJumps;
    [Space(3)]

    [Header("Attack Settings - Pick")]
    [SerializeField] private bool attack = false;
    [SerializeField] private float timeBetweenAttack, timeSinceAttack;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY;
    [SerializeField] private float groundCheckX;
    [SerializeField] private LayerMask whatIsGround;
    [Space(3)]

    //References
    public ProjectileBehavior ProjectilePrefab;
    public Transform LaunchOffset;
    private float xAxis;
    private float gravity;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerStates pState;

    public static PlayerController Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        pState = GetComponent<PlayerStates>();

        gravity = rb.gravityScale;

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        UpdateJump();

        if(pState.dashing) return;
        Flip();
        Move();
        Jump();
        StartDash();
        Pick();
        
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        attack = Input.GetButtonDown("Pick");
    }

    void Flip()
    {
        if(xAxis < 0)
        {
            //sprite.flipX = true;
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if(xAxis > 0)
        {
            //sprite.flipX = false;
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if(Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if(Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        if(Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        anim.ResetTrigger("Dashing");
    }

    void Pick()
    {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack >= timeBetweenAttack){
            timeSinceAttack = 0;
            Instantiate(ProjectilePrefab, LaunchOffset.position, transform.rotation);
        }
        anim.SetBool("Attacking", attack);
    }

    void Jump()
    {
       if(Input.GetButtonUp("Jump") && rb.velocity.y > 0)
       {
            pState.jumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
       }

       if(!pState.jumping)
       {
            if(jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
             
                pState.jumping = true;
            }
            else if(!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;

                airJumpCounter++;

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
       }
        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJump()
    {
        if(Grounded())
        {
            coyoteTimeCounter = coyoteTime;
            pState.jumping = false;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if(Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    public bool Grounded()
    {
        if(Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
        || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX,0,0), Vector2.down, groundCheckY, whatIsGround)
        || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX,0,0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
