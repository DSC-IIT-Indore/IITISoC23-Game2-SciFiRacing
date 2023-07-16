using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("Ship Settings")]
    public float acceleration = 5000f;
    public float deceleration = 1000f;
    public float minSpeed = 200f;
    public float maxSpeed = 400f;

    public float dragCoefficient = 1f;
    
    [Header("Input Settings")]
    private bool accelInput;
    private float vertical, horizontal;
    private float verticalDisplacement = 0f, horizontalDisplacement = 0f;
    public float maxVerticalDisplacement = 30f, maxHorizontalDisplacement = 30f;
    public float horizontalSpeed = 2.0f, verticalSpeed = 2.0f;
    public float maxHorizontalDelta = 1f, maxVerticalDelta = 1f;
    private float lastHorizontal = 0, lastVertical = 0;
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
        vertical = Input.GetAxis("Mouse Y");
        horizontal = Input.GetAxis("Mouse X");
        accelInput = Input.GetKey(KeyCode.LeftShift);

        verticalDisplacement = Mathf.Clamp(verticalDisplacement + vertical, -maxVerticalDisplacement, maxVerticalDisplacement);
        horizontalDisplacement = Mathf.Clamp(horizontalDisplacement + horizontal, -maxHorizontalDisplacement, maxHorizontalDisplacement);

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
