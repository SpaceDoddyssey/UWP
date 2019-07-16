using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigWheelController : MonoBehaviour
{
    private float drag = 0.98f;
    private float brakeDrag = 0.7f;

    private float velocity;
    private float speedBoost = 10.0f;
    private float topSpeed = 700;
    public SeatController seat;
  
    private float lastRot, curRot;
    Quaternion oldPos, newPos;
    private bool isGrabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        lastRot = transform.rotation.eulerAngles.x;
        curRot = transform.rotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(velocity > topSpeed){
            velocity = topSpeed;
            Debug.Log("Top speed exceeded");
        }
        if(!isGrabbed){
            velocity *= drag;
        }
        isGrabbed = false;
        //curRot = transform.rotation.eulerAngles.x;
        turnWheel(velocity * Time.deltaTime);
     }
    
    public void addForce(float force) {
        velocity += force * speedBoost;
        isGrabbed = true;
        //Debug.Log(force * Time.deltaTime);
    }

    public void turnWheel(float degrees) {
        curRot += degrees;
        //Debug.Log("Degrees: " + degrees);
        if (curRot >= 360) { curRot -= 360.0f; lastRot -= 360.0f; }
        if (curRot < 0) { curRot += 360.0f; lastRot += 360.0f; }
        transform.Rotate(degrees, 0.0f, 0.0f);
        //transform.localRotation = Quaternion.Euler(wheelSpeed, 0.0f, 0.0f);
    }

    void OnCollisionEnter(Collision col) {
        seat.CollisionHandle(col);
    }

    public void brake() {
        velocity *= brakeDrag;
        //velocity *= brakeDrag * Time.deltaTime;
    }

    public void stopWheel() {
        velocity = 0.0f;
    }

    public float deltaRot() { // Returns the number of degrees the wheel has rotated since last polled
        float delta = curRot - lastRot;
        //Debug.Log("LastRot " + lastRot + "  -  curRot " + curRot + "  -  delta " + delta);
        lastRot = curRot;
        return delta;
    }
}
