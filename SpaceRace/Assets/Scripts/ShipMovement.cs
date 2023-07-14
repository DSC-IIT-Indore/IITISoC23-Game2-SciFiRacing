using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("Ship Settings")]
    public float acceleration = 5000f;
    public float deceleration = 1000f;
    public float minSpeed = 200f;
    public float maxSpeed = 400f;

    public float dragCoefficient = 1f;
    
    private bool accelInput;
    private float vertical, horizontal;
    public float horizontalSpeed = 2.0f, verticalSpeed = 2.0f;
    public float tiltSpeed = 2.0f;

    public Transform playerModel;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * minSpeed;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        vertical = Input.GetAxis("Mouse Y") * verticalSpeed;
        horizontal = Input.GetAxis("Mouse X") * horizontalSpeed;
        accelInput = Input.GetKey(KeyCode.LeftShift);

        Vector3 targetRotation = new Vector3(vertical, horizontal/2f, -horizontal);
        Vector3 rotation = Vector3.Lerp(Vector3.zero, targetRotation, tiltSpeed * Time.deltaTime);
        transform.Rotate(rotation);
    }

    void FixedUpdate()
    {
        float velMag = rb.velocity.magnitude;

        if(velMag < minSpeed){
            Vector3 forceDir = transform.forward * acceleration * Time.fixedDeltaTime;
            rb.AddForce(forceDir, ForceMode.Acceleration);
        }
        if(accelInput && velMag < maxSpeed){
            Vector3 forceDir = transform.forward * acceleration * Time.fixedDeltaTime;
            rb.AddForce(forceDir, ForceMode.Acceleration);

        }else if(velMag > minSpeed * 1.1f){
            Vector3 forceDir = -rb.velocity.normalized * deceleration * Time.fixedDeltaTime;
            rb.AddForce(forceDir, ForceMode.Acceleration);
        }

        rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * velMag, dragCoefficient*Time.fixedDeltaTime);
        
    }
}
