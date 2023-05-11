using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour{

    private Rigidbody2D body2D;
    private SpriteRenderer sprite;

    [Header("Stats")]
    public float speed = 25.0f;
    public float airControl = 0.8f;
    public float jumpPower = 8.0f;

    [Header("States")]
    public bool inAir = false;


    // Start is called before the first frame update
    void Start(){
        body2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update(){
        Move();
        Jump();
        
    }

    void Move(){
        //Tracking which direction the player would like to go
        Vector2 horizontalMove = new Vector2(Input.GetAxis("Horizontal"), 0);

        //Checking if the player is in the air
        if(inAir){
            horizontalMove *= airControl;
        }

        //Moving the player
        body2D.position += horizontalMove * speed * Time.deltaTime;

        //Making sure the player sprite is facing the right way
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
            sprite.flipX = true;
        }
        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
            sprite.flipX = false;
        }
    }

    void Jump(){
        //checking player input
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && !inAir){
            //making the playre jump
            body2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            inAir = true;
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        inAir = false;
    }
}
