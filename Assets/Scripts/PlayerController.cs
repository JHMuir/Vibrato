using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [Space]
    public new Rigidbody2D rigidbody; //Reference to the character's Rigidbody component
    public Animator animator; //Reference to the character's Animator component

    [Header("Player Movement Settings")]
    [Space]
    [SerializeField] private float jumpForce = 100f;    //Amount of force added when the player jumps
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;    //How much to smooth out the movement
    public float runSpeed = 40f;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    //[SerializeField] private float coyoteTime;
    //private float coyoteTimeCounter;
    private int airJumpCounter  = 0;
    [SerializeField] private int maxAirJumps = 1;
    
    [Header("Ground Check Settings")]
    [Space]
    [SerializeField] private LayerMask whatIsGround;    //A mask determing what is ground to the character 
    [SerializeField] private Transform groundCheck;     //A position marking where to check if the player is grounded
    [SerializeField] private Transform ceilingCheck;    //A position marking where to check for ceilings
    
    //Private Variables
    const float groundedRadius = .2f;   //Radius of the overlap circle to determing if grounded
    const float ceilingRadius = .2f;    //Radius of the overlap circle to determine if the player can stand up
    private float horizontalMove = 0;
    private bool jumpInput = false;
    private bool isDashing = false;
    private bool dashInput = false;
    private bool canDash = true;
    private bool dashed;
    private bool onAttack;
    private float gravity;
    private bool facingRight = true;    //For determining which way the player is currently facing
    private Vector3 velocity = Vector3.zero;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gravity = rigidbody.gravityScale;
    }

    void Update()
    {
        GetInput();
        Debug.Log(airJumpCounter);
        Debug.Log(jumpInput);
    }

    void FixedUpdate()
    {
        if(isDashing) return;
        Jump(jumpInput);
        jumpInput = false;
        Move(horizontalMove * Time.fixedDeltaTime);
        StartDash(dashInput);
    }

    void GetInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if(Input.GetButtonDown("Jump")) jumpInput = true;
        if(Input.GetButtonUp("Jump")) jumpInput = false;
        if(Input.GetButtonDown("Dash") && canDash && !dashed)  dashInput = true;
    }

    public bool Grounded()
    {
        bool grounded = false;

        //Character is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for(int i = 0; i < colliders.Length; i++){
            if(colliders[i].gameObject != gameObject) grounded = true;
        }
        return grounded;
    }

    void Move(float move)
    {
        if(Grounded())
        {
            //Move the character by finding the target velocity 
            Vector3 targetVelocity = new Vector3(move * 10f, rigidbody.velocity.y);
            //And then smoothing it out and applying it to the character
            rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, targetVelocity, ref velocity, movementSmoothing);
        }
        else if(!Grounded())
        {
            //If in the air, we keep control but lose some speed 
            Vector3 targetVelocity = new Vector3((move * 10f) * .75f, rigidbody.velocity.y);
            //And then smoothing it out and applying it to the character
            rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, targetVelocity, ref velocity, movementSmoothing);
        }
        //If the input is moving the character right and facing left
        if(move > 0 && !facingRight) Flip();
        //otherwise, character is moving left and facing right
        else if(move < 0 && facingRight) Flip();

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    void Jump(bool jump){
        if(jump && airJumpCounter < maxAirJumps)
        {
            rigidbody.AddForce(new Vector2(0f, jumpForce));
            Debug.Log("Here");
            airJumpCounter++;      
        }
        if(Grounded())
        {
            jumpInput = false;
            airJumpCounter = 0;
        }
        animator.SetBool("Jumping", !Grounded());
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
        animator.SetTrigger("Dashing");
        rigidbody.gravityScale = 0;
        if(facingRight) rigidbody.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        else rigidbody.velocity = new Vector2(-transform.localScale.x * dashSpeed, 0);
        if(Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rigidbody.gravityScale = gravity;
        dashInput = false;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        animator.ResetTrigger("Dashing");
    }
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
