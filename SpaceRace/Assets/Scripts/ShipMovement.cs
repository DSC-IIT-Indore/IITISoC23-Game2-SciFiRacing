using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float acceleration = 10f;
    public float maxSpeed = 100f;
    public float maxRollAngle = 30f;
    public float maxPitchAngle = 30f;
    public float tiltConstant = 10f;
    public float dragCoefficient = 1f;
    public float pitchSpeed = 2f;  
    public float yawSpeed = 2f;    
    public float rollSpeed = 2f;
    public float liftForce = 10f;


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
            Vector3 forceDir = transform.forward * accelInput * acceleration * Time.fixedDeltaTime;
            rb.AddForce(forceDir, ForceMode.Force);
        }

        Vector3 lift = transform.up * -pitchInput * liftForce * Vector3.Dot(transform.forward, rb.velocity) * Time.fixedDeltaTime;
        rb.AddForce(lift, ForceMode.Force);

        ;//rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * rb.velocity.magnitude, dragCoefficient*Time.fixedDeltaTime);


        Quaternion pitchRotation = Quaternion.Euler(pitchInput * pitchSpeed * Time.fixedDeltaTime, 0f, 0f);
        Quaternion yawRotation = Quaternion.Euler(0f, yawInput * yawSpeed * Time.fixedDeltaTime, 0f);
        Quaternion rollRotation = Quaternion.Euler(0f, 0f, -rollInput * rollSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation * pitchRotation * yawRotation * rollRotation);
        
    }
}
