using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float acceleration = 10f;
    public float MAX_SQR_SPEED = 2500f;
    private Rigidbody rb;
    private Vector3 inputData = Vector3.zero;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float z = Input.GetButtonDown("L Shift") ? 1 : 0;
        inputData = new Vector3(h, v, z);
    }

    void FixedUpdate()
    {
        if(inputData.z == 1 && rb.velocity.sqrMagnitude < MAX_SQR_SPEED){
            rb.AddForce(transform.forward * acceleration);
        }
    }
}
