using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("Ship Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 100f;
    public float maxRollAngle = 0.1f;
    public float maxPitchAngle = 0.1f;
    public float tiltConstant = 10f;
    public float pitchSpeed = 2f;  
    public float yawSpeed = 2f;    
    public float rollSpeed = 2f;
    public float liftForce = 10f;
    public float dragCoefficient = 1f;
    
    private float pitchInput, yawInput, rollInput, accelInput;
    private Rigidbody rb;
    private Quaternion pitchRotation, yawRotation, rollRotation;
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
        float velMag = rb.velocity.magnitude;
        if(velMag < maxSpeed){
            // Vector3 forceDir = transform.forward * accelInput * acceleration * Time.fixedDeltaTime;
            Vector3 forceDir = transform.forward * acceleration * Time.fixedDeltaTime;
            rb.AddForce(forceDir, ForceMode.Force);
        }
 
        Vector3 lift = transform.up * transform.forward.y * liftForce * Vector3.Dot(transform.forward, rb.velocity) * Time.fixedDeltaTime;
        rb.AddForce(lift, ForceMode.Force);

        rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * velMag, dragCoefficient*Time.fixedDeltaTime);

        pitchInput *= velMag;
        yawInput *= velMag;
        rollInput *= velMag;
        
        pitchRotation = transform.forward.y*-pitchInput <= maxPitchAngle && pitchInput != 0 
                        ? Quaternion.Euler(pitchInput * pitchSpeed * Time.fixedDeltaTime, 0f, 0f)
                        : Quaternion.Lerp(pitchRotation, Quaternion.identity, tiltConstant*Time.fixedDeltaTime);
        
        yawRotation = Quaternion.Euler(0f, yawInput * yawSpeed * Time.fixedDeltaTime, 0f);
        rollRotation = Quaternion.Euler(0f, 0f, -rollInput * rollSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation * pitchRotation * yawRotation * rollRotation);
        
    }
}
