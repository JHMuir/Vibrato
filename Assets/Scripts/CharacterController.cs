using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{
    [Header("Settings")]
    [Space]
    [SerializeField] private float jumpForce = 400f;    //Amount of force added when the player jumps
    //[Range(0,1)] [SerializeField] private float crouchSpeed = .36f;     //Amount of maxSpeed applied to crouching movement. 1 = 100% 
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;    //How much to smooth out the movement
    [SerializeField] private bool airControl = false;   //Whether or not a character can steer while jumping

    [Header("Ground Check Settings")]
    [Space]
    [SerializeField] private LayerMask whatIsGround;    //A mask determing what is ground to the character 
    [SerializeField] private Transform groundCheck;     //A position marking where to check if the player is grounded
    [SerializeField] private Transform ceilingCheck;    //A position marking where to check for ceilings
    [SerializeField] private Collider2D crouchDisableCollider;     //A collider that will be disabled when crouching

    //References
    const float groundedRadius = .2f;   //Radius of the overlap circle to dterming if grounded
    private bool grounded;  //Whether or not the character is grounded
    const float ceilingRadius = .2f;    //Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D rb;     //Reference to the character's Rigidbody component
    private bool facingRight = true;    //For determining which way the player is currently facing
    private Vector3 velocity = Vector3.zero;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>{}

    //public BoolEvent OnCrouchEvent;
    //private bool wasCrouching = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if(OnLandEvent == null) OnLandEvent = new UnityEvent();
        
        //if(OnCrouchEvent == null) OnCrouchEvent = new BoolEvent();
    
    }

    private void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = false;

        //Character is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for(int i = 0; i < colliders.Length; i++){
            if(colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if(!wasGrounded) OnLandEvent.Invoke();
                
            }
        }
    }

    public void Move(float move)//, bool crouch)
    {
        

        //only control the player if grounded or airControl is turned on
        if(grounded || airControl){
            

            //Move the character by finding the target velocity 
            Vector3 targetVelocity = new Vector3(move * 10f, rb.velocity.y);
            //And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

            //If the input is moving the character right and facing left
            if(move > 0 && !facingRight) Flip();
            //otherwise, character is moving left and facing right
            else if(move < 0 && facingRight) Flip();
        }
    }

    public void Jump(bool jump)
    {
        //If character should jump...
        if(grounded && jump)
        {
            grounded = false;
            rb.AddForce(new Vector2(0f, jumpForce));
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
