using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatController : MonoBehaviour // Seat Controller as of 6/2/19, 
{
    public BigWheelController[] wheels = new BigWheelController[2]; // wheels[0] = left,  wheels[1] = right
    public SmallWheelController[] smallWheels = new SmallWheelController[2]; // same, [0] = left, [1] = right
    public GameObject chair, chairRig;
    public MeshCollider seatCollider;
    Vector3 leftWheelPos, rightWheelPos;
    private float wheelHeight, circumference, axleLength, chairCircumference;
    private float[] moveDelta = new float[2]; // The distance the wheel has rolled since the last update - Similarly, [0] = left, [1] = right
    private bool frontStop, backStop;

    void Awake() {
        MeshRenderer wheelModel = wheels[0].gameObject.GetComponent<MeshRenderer>();

        wheelHeight = wheelModel.bounds.size.y;
        //Debug.Log("Wheel height is " + wheelHeight);
        float radius = wheelHeight / 2.0f;
        circumference = (2.0f * Mathf.PI * radius);

        //Time.timeScale = 0.1f;
    }

    void Start() {
        //chair = GameObject.Find("Wheelchair");
        //chair = GetComponent<Rigidbody>();
        initWheelInfo();

        Vector3 centerPos = (leftWheelPos + rightWheelPos) / 2.0f;
        Vector3 chairPos = chair.transform.position;

        chairCircumference = 2.0f * axleLength * Mathf.PI; // The circumference of the circle the chair makes when rotated about one wheel
    }

    void initWheelInfo() {

        leftWheelPos = wheels[0].gameObject.transform.position;
        rightWheelPos = wheels[1].gameObject.transform.position;

        Vector3 axle = leftWheelPos - rightWheelPos;
        axleLength = axle.magnitude;
    } 

    void Update() {
        checkInput();
        for (int i = 0; i < 2; i++) {
            moveDelta[i] = wheels[i].deltaRot() * (circumference / 360.0f);
        }
        //Debug.Log(moveDelta[0] + "    " + moveDelta[1]);
        float d0 = moveDelta[0];
        float d1 = moveDelta[1];
        if (!(d0 == 0 && d1 == 0)) {
            float commonDist = Mathf.Min(d0, d1);
            float greaterDist = Mathf.Max(d0, d1);
            float remainderDist = greaterDist - commonDist;
            //Debug.Log("d0 " + d0 + " d1 " + d1);
            bool didMove = MoveChair(commonDist + remainderDist / 2.0f);

            if(didMove){
                smallWheels[0].turnWheel(d0);
                smallWheels[1].turnWheel(d1);

                //wheels[0].turnWheel((d0 / circumference) * 360.0f);
                //wheels[1].turnWheel((d1 / circumference) * 360.0f);

                float angle = (remainderDist / chairCircumference) * 360.0f;
                RotateChair(d0 > d1 ? angle : -angle);
            }
        }
    }


    public float speedMod;
    private void checkInput() {
        float leftStick = Input.GetAxis("LeftVertical");
        float rightStick = Input.GetAxis("RightVertical");

        float leftTurn = speedMod * leftStick * Time.deltaTime;
        wheels[0].turnWheel(leftTurn);
        float rightTurn = speedMod * rightStick * Time.deltaTime;
        wheels[1].turnWheel(rightTurn);

        //Debug.Log("Left turn: " + leftTurn);
    }

    void OnCollisionEnter(Collision col) {
        CollisionHandle(col);
    }

    public void CollisionHandle(Collision col) {
        //Debug.Log("I hit " + col.gameObject.name);
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Door") {
            Debug.Log("Collided with a wall");
            wheels[0].stopWheel();
            wheels[1].stopWheel();
            Vector3 colVector = col.GetContact(0).point - chairRig.transform.position;
            colVector.Normalize();
            float angle = Vector3.Dot(colVector, chairRig.transform.forward);
            if (angle > 0) { frontStop = true; backStop = false; }
            else { frontStop = false; backStop = true; }
        }
        //GameObject colTracker = GameObject.Find("CollisionTracker");
        //colTracker.transform.position = col.GetContact(0).point;
    }

    private bool MoveChair(float distance) {
        if(distance < 0){
            frontStop = false;
            if (backStop) {
                wheels[0].stopWheel();
                wheels[1].stopWheel();
                return false;
            } else {
                chair.transform.position += chair.transform.forward * distance;
            }
        } else {
            backStop = false;
            if (frontStop) {
                wheels[0].stopWheel();
                wheels[1].stopWheel();
                return false;
            }
            else {
                chair.transform.position += chair.transform.forward * distance;
            }
        }
        return true;
        //Debug.Log("Moving chair " + distance);
    }

    private void RotateChair(float angle) {
        Vector3 eulers = new Vector3(0.0f, angle, 0.0f);
        chair.transform.Rotate(eulers);
    }
}
