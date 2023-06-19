using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float acceleration = 10f;
    public float MAX_SPEED = 100f;
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
        float z = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;
        inputData = new Vector3(h, v, z);
    }

    void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude < MAX_SPEED*MAX_SPEED){
            rb.velocity += transform.forward * acceleration * inputData.z * Time.fixedDeltaTime;
        }

        
    }
}
