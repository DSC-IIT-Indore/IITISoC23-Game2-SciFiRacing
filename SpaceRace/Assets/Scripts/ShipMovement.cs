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
    public float turnSpeed = 2.0f;
    public float maxHorizontalAngularSpeed = 10f;
    public float maxVerticalAngularSpeed = 10f;
    public bool getMousePos = false;

    private bool accelInput;
    private Vector3 mousePos;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * minSpeed;

        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ActivateInput()
    {
        Cursor.lockState = CursorLockMode.Locked;
        getMousePos = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    public void DeactivateInput()
    {
        Cursor.lockState = CursorLockMode.Locked;
        getMousePos = false;
    }

    void Update()
    {
        // Get input
        if(getMousePos) mousePos = Input.mousePosition - new Vector3(Screen.width/2f, Screen.height/2f, 0f);
        else mousePos = Vector3.Lerp(mousePos, Vector3.zero, turnSpeed * Time.deltaTime);

        // Activate or deactivate input
        if(Input.GetKeyDown(KeyCode.LeftControl)) ActivateInput();
        if(Input.GetKeyDown(KeyCode.LeftAlt)) DeactivateInput();

        accelInput = Input.GetKey(KeyCode.LeftShift);
        
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

        // Rotate ship
        float horizontal = (mousePos.x / Screen.width) * 2f * maxHorizontalAngularSpeed * Time.fixedDeltaTime;
        float vertical = (mousePos.y / Screen.height) * 2f * maxVerticalAngularSpeed * Time.fixedDeltaTime;
        Vector3 targetRotation = new Vector3(vertical, horizontal/2f, -horizontal);
        Vector3 rotation = Vector3.Lerp(Vector3.zero, targetRotation, turnSpeed * Time.fixedDeltaTime);
        transform.Rotate(rotation);

        // Change the velocity direction to match the ship's forward direction
        rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * velMag, dragCoefficient*Time.fixedDeltaTime);
        
    }
}
