using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("Ship Settings")]
    public float acceleration = 10f, deceleration = 10f;
    public float minSpeed = 200f;
    public float maxSpeed = 100f;

    public float dragCoefficient = 1f;
    
    private float accelInput;
    private float vertical, horizontal;
    public float horizontalSpeed = 2.0f, verticalSpeed = 2.0f;

    public Transform playerModel;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * minSpeed;
        
    }

    void Update()
    {
        vertical = Input.GetAxis("Mouse Y") * verticalSpeed;
        horizontal = Input.GetAxis("Mouse X") * horizontalSpeed;
        accelInput = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;

        Vector3 rotation = new Vector3(vertical, horizontal/2f, -horizontal);
        transform.Rotate(rotation);
    }

    void FixedUpdate()
    {
        float velMag = rb.velocity.magnitude;

        if(velMag < minSpeed){
            Vector3 forceDir = transform.forward * acceleration * Time.fixedDeltaTime;
            rb.AddForce(forceDir, ForceMode.Acceleration);
        }
        if(accelInput==1f && velMag < maxSpeed){
            Vector3 forceDir = transform.forward * acceleration * Time.fixedDeltaTime;
            rb.AddForce(forceDir, ForceMode.Acceleration);
        }else if(velMag > minSpeed * 1.1f){
            Vector3 forceDir = -rb.velocity.normalized * deceleration * Time.fixedDeltaTime;
            rb.AddForce(forceDir, ForceMode.Acceleration);
        }
 
        rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * velMag, dragCoefficient*Time.fixedDeltaTime);

    }
}
