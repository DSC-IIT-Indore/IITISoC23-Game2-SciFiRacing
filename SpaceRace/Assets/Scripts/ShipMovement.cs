using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float acceleration = 10f;
    public float MAX_SPEED = 100f;
    public float max_roll_angle = 30f;
    public float max_pitch_angle = 30f;
    public float tilt_constant = 10f;
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

        if(inputData.y != 0){
            Vector3 target = Vector3.RotateTowards(transform.forward, transform.up*inputData.y, 
                                                      max_pitch_angle*Mathf.Deg2Rad, 0);
            
            transform.forward = Vector3.Lerp(transform.forward, target, tilt_constant*Time.fixedDeltaTime);
        }

    }
}
