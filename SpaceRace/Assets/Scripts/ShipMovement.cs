using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float acceleration = 10f;
    public float maxSpeed = 100f;
    public float maxRollAngle = 30f;
    public float maxPitchAngle = 30f;
    public float tilt_constant = 10f;
    public float pitchSpeed = 2f;  
    public float yawSpeed = 2f;    
    public float rollSpeed = 2f;  
    private float pitchInput;      
    private float yawInput;       
    private float rollInput;
    private float accelInput;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        pitchInput = Input.GetAxis("Vertical");
        yawInput = Input.GetAxis("Horizontal");
        rollInput = Input.GetAxis("Roll");
        accelInput = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;
    }

    void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude < maxSpeed*maxSpeed){
            rb.velocity += transform.forward * acceleration * accelInput  * Time.fixedDeltaTime;
        }

        Quaternion pitchRotation = Quaternion.Euler(pitchInput * pitchSpeed, 0f, 0f);
        Quaternion yawRotation = Quaternion.Euler(0f, yawInput * yawSpeed, 0f);
        Quaternion rollRotation = Quaternion.Euler(0f, 0f, -rollInput * rollSpeed);

        rb.MoveRotation(rb.rotation * pitchRotation * yawRotation * rollRotation);

    }
}
