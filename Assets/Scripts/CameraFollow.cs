using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Variables")]
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] GameObject Player; 


    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Player.transform.position + offset, followSpeed);

    }
}
