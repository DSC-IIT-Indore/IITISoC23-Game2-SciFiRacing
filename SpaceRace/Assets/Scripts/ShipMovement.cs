using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    // Ship settings
    [Header("Ship Settings")]
    public float acceleration = 5000f;
    public float deceleration = 1000f;
    public float minSpeed = 200f;
    public float maxSpeed = 400f;
    public float dragCoefficient = 1f;

    // Input settings 
    [Header("Input Settings")]
    public float maxVerticalDisplacement = 30f;
    public float maxHorizontalDisplacement = 30f;
    public float horizontalSpeed = 2.0f, verticalSpeed = 2.0f;
    public float maxHorizontalDelta = 1f, maxVerticalDelta = 1f;
    public float tiltSpeed = 2.0f;
    private float verticalDisplacement = 0f, horizontalDisplacement = 0f;
    private bool accelInput;
    private float vertical, horizontal;
    private float lastHorizontal = 0, lastVertical = 0;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * minSpeed;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get input
        vertical = Input.GetAxis("Mouse Y");
        horizontal = Input.GetAxis("Mouse X");
        accelInput = Input.GetKey(KeyCode.LeftShift);

        // Calculate displacement
        verticalDisplacement = Mathf.Clamp(verticalDisplacement + vertical, -maxVerticalDisplacement, maxVerticalDisplacement);
        horizontalDisplacement = Mathf.Clamp(horizontalDisplacement + horizontal, -maxHorizontalDisplacement, maxHorizontalDisplacement);

        // Calculate rotation
        if(Mathf.Abs(verticalDisplacement) >= maxVerticalDisplacement){
            vertical = lastVertical;
            vertical = Mathf.Clamp(vertical, -maxVerticalDelta, maxVerticalDelta);
        }
        
        if(Mathf.Abs(horizontalDisplacement) >= maxHorizontalDisplacement){
            horizontal = lastHorizontal;
            horizontal = Mathf.Clamp(horizontal, -maxHorizontalDelta, maxHorizontalDelta);
        }

        lastHorizontal = horizontal != 0 ? horizontal : lastHorizontal;
        lastVertical = vertical != 0 ? vertical : lastVertical;

        vertical *= verticalSpeed;
        horizontal *= horizontalSpeed;

        // Rotate ship
        Vector3 targetRotation = new Vector3(vertical, horizontal/2f, -horizontal);
        Vector3 rotation = Vector3.Lerp(Vector3.zero, targetRotation, tiltSpeed * Time.deltaTime);
        transform.Rotate(rotation);
    }

    void FixedUpdate()
    {
        // Calculate velocity
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

        // Change the velocity direction to match the ship's forward direction
        rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * velMag, dragCoefficient*Time.fixedDeltaTime);
        
    }
}
