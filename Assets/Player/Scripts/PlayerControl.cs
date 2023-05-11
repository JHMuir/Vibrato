using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour{

    private Rigidbody2D body2D;
    private SpriteRenderer sprite;

    [Header("Stats")]
    public float speed = 25.0f;
    public float airControl = 0.5f;
    public float jumpPower = 100.0f;


    // Start is called before the first frame update
    void Start(){
        body2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update(){
        Move();
    }

    void Move(){
        Vector2 horizontalMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        body2D.position += horizontalMove * speed * Time.deltaTime;
        //float moveForce = horizontalMove * speed * Time.deltaTime;

        //body2D.AddForce(Vector2.right * moveForce, ForceMode2D.Impulse);

        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
            sprite.flipX = true;
        }
        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
            sprite.flipX = false;
        }
    }
}
