using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guitar : MonoBehaviour
{

    public Transform firePoint;
    public GameObject notePrefab;

    public float attackInterval = 0.2f;
    private float nextNote = 0.0f;
    void Update()
    {
        if(Input.GetButtonDown("Pick") && Time.time >= nextNote)
        {
            nextNote = Time.time + attackInterval;
            Invoke("Pick", .2f);
        }
    }

    void Pick()
    {
        Instantiate(notePrefab, firePoint.position, firePoint.rotation);
    }
}
