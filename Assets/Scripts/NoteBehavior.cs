using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehavior : MonoBehaviour
{
    public float speed = 10.0f;
    public float liveTime = 5f;
    public Rigidbody2D rb;
    void Start()
    {
        rb.velocity = transform.right * speed;
        Destroy(gameObject, liveTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log(hitInfo.name);
        if(hitInfo.name == "Aria" | hitInfo.name == "Note(Clone)")
        {
            //do nothing
        }
        else{
            Destroy(gameObject);
        }
    }
}
