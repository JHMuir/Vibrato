using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class CharacterController : MonoBehaviour
{

    [Header("References")]
    protected Rigidbody2D rb;     //Reference to the character's Rigidbody component
    [Space]

    [Header("Character Movement Settings")]
    [SerializeField] protected float jumpForce = 200f; 
    [SerializeField] protected float runSpeed = 40f;  //Amount of force added when the player jumps
    [SerializeField] protected float airSpeedReduction = .75f; //Holds the amount 
    [Range(0, .3f)] [SerializeField] protected float movementSmoothing = .05f;    //How much to smooth out the movement
    [Space]

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask whatIsGround;    //A mask determing what is ground to the character 
    [SerializeField] private Transform groundCheck;     //A position marking where to check if the player is grounded
    [Space]

    //Private Variables
    const float groundedRadius = .2f;   //Radius of the overlap circle to dterming if grounded
    [HideInInspector]
    public bool facingRight = true;    //For determining which way the player is currently facing
    protected Vector3 velocity = Vector3.zero;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual bool Grounded()
    {
        bool grounded = false;

        //Character is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for(int i = 0; i < colliders.Length; i++){
            if(colliders[i].gameObject != gameObject) grounded = true;
        }
        return grounded;
    }

    protected virtual void Move(float move)
    {
        //only control the player if grounded or airControl is turned on
        if(Grounded()){
            //Move the character by finding the target velocity 
            Vector3 targetVelocity = new Vector3(move * 10f, rb.velocity.y);
            //And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        }
        //If the input is moving the character right and facing left
        if(move > 0 && !facingRight) Flip();
        //otherwise, character is moving left and facing right
        else if(move < 0 && facingRight) Flip();
    }

    protected virtual void Jump(bool jump)
    {
        
        rb.AddForce(new Vector2(0f, jumpForce));
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
