using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Engine based stats
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1;

    [Header("Jump Settings: ")]
    [SerializeField] private float jumpForce = 20; 
    [SerializeField] private int jumpBufferFrames;
    private int jumpBufferCounter = 0;
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;

    [Header("Ground Check Settings: ")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY;
    [SerializeField] private float groundCheckX;
    [SerializeField] private LayerMask whatIsGround;

    //References
    private float xAxis;
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

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        UpdateJump();
        Move();
        Jump();
        
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());

        if(xAxis < 0)
        {
            sprite.flipX = true;
        }
        else if(xAxis > 0)
        {
            sprite.flipX = false;
        }
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
                pState.jumping = true;
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
