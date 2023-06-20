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

        float target_pitch_angle = Mathf.LerpAngle(transform.rotation.x, max_pitch_angle*inputData.y, tilt_constant);
        float target_roll_angle = Mathf.LerpAngle(transform.rotation.z, max_roll_angle*inputData.x, tilt_constant);

        
    }
}
