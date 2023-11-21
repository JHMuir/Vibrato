using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guitar : MonoBehaviour
{

    public Transform firePoint;
    public GameObject notePrefab;
    void Update()
    {
        if(Input.GetButtonDown("Pick"))
        {
            Pick();
        }
    }

    void Pick()
    {
        Instantiate(notePrefab, firePoint.position, firePoint.rotation);
    }
}
