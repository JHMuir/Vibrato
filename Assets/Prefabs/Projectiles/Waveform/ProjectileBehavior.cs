using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 5.0f;
    // Update is called once per frame
    void Update()
    {
        transform.position += -transform.right * Time.deltaTime * speed;
    }

    void OnCollisionEnter2D(Collision2D collision){
        Destroy(gameObject);
    }
}
