using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSeatController : MonoBehaviour // Seat Controller as of 6/2/19, just implemented discrete turning formula 
{
    public WheelController[] wheels = new WheelController[2]; // wheels[0] = left  :  wheels[1] = right
    public GameObject chair, chairRig;
    //private Rigidbody chair;
    Vector3 leftWheelPos, rightWheelPos;
    private float wheelHeight, circumference, axleLength, chairCircumference;
    private float[] moveDelta = new float[2]; // The distance the wheel has rolled since the last update - Similarly, [0] = left, [1] = right


    void Start() {
        //chair = GameObject.Find("Wheelchair");
        //chair = GetComponent<Rigidbody>();
        wheels[0].speed = 1.0f;
        wheels[1].speed = 1.5f;
        MeshRenderer wheel = wheels[0].gameObject.GetComponent<MeshRenderer>();

        wheelHeight = wheel.bounds.size.y;
        Debug.Log("Wheel height is " + wheelHeight);
        float radius = wheelHeight / 2.0f;
        circumference = (2.0f * Mathf.PI * radius);

        leftWheelPos = wheels[0].gameObject.transform.position;
        rightWheelPos = wheels[1].gameObject.transform.position;
        Vector3 axle = leftWheelPos - rightWheelPos;
        axleLength = axle.magnitude;

        Vector3 centerPos = (leftWheelPos + rightWheelPos) / 2.0f;
        Vector3 chairPos = chair.transform.position;

        chairCircumference = 2.0f * axleLength * Mathf.PI;
    }

    void Update() {
        for (int i = 0; i < 2; i++) {
            moveDelta[i] = wheels[i].deltaRot() * (circumference / 360.0f);
        }
        Debug.Log(moveDelta[0] + "    " + moveDelta[1]);
        float d0 = moveDelta[0];
        float d1 = moveDelta[1];
        if (d0 == 0 && d1 == 0) {
            Debug.Log("No movement necessary");
        }
        else {// if (d0 >= 0 && d1 >= 0) { // Both wheels moving forwards
              /*
              //Galen: "If you have wheel speeds VR and VL in m/s and radius R in m, instantaneous forward velocity  V = (VL + VR)/2 and instantaneous angular velocity w = (VR - VL)/2R [in rad/s]"
              float forwardVel = (d0 + d1) / 2.0f;
              float angularVel = (d1 - d0) / (2.0f * (axleLength/2.0f));
              angularVel *= Mathf.Rad2Deg * Time.deltaTime;

              angularVel = 90f * Time.deltaTime;
              Vector3 curRot = new Vector3(0.0f, angularVel, 0.0f);  // chair.transform.rotation.eulerAngles;

              float newXPos = chair.transform.position.x + (forwardVel * Mathf.Cos(chair.transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) * Time.deltaTime;
              float newZPos = chair.transform.position.z + (forwardVel * Mathf.Cos(chair.transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) * Time.deltaTime;

              chair.transform.Rotate(curRot);
              chair.transform.position = new Vector3(newXPos, chair.transform.position.y, newZPos);

              /* Galen: 
              If you have absolute rotation Theta and absolute position (x, y), for a small timestep dt you can find your new position and rotation with:
                  Theta' = Theta + w*dt
                  x' = x + V*cos(Theta)*dt
                  y' = y + V*sin(Theta)*dt

              */

            float commonDist = Mathf.Min(d0, d1);
            float greaterDist = Mathf.Max(d0, d1);
            float remainderDist = greaterDist - commonDist;
            MoveChair(commonDist + remainderDist / 2.0f);

            float angle = (remainderDist / chairCircumference) * 360.0f;
            RotateChair(d0 > d1 ? angle : -angle);
            //RotateChair(angle);
            /*
            //float angle = Mathf.Asin(axleLength / (remainderDist / 2.0f)); // BLOWS UP IF THE REMAINDER DIST IS ZERO
            leftWheelPos = wheels[0].gameObject.transform.position;
            rightWheelPos = wheels[1].gameObject.transform.position;
            Vector3 pivotPos = (d0 > d1 ? rightWheelPos : leftWheelPos);
            Vector3 movingWheel = (d0 > d1 ? leftWheelPos : rightWheelPos); // The position of whichever wheel is not the pivot

            float newWheelX = movingWheel.x + axleLength * Mathf.Sin(angle);
            Debug.Log("NewWheelX - " + movingWheel.x + " --- " + axleLength + " --- " + angle);
            float newWheelZ = movingWheel.z - axleLength * (1 - Mathf.Cos(angle)); // The new coordinates of the wheel, source: https://math.stackexchange.com/questions/332743/calculating-the-coordinates-of-a-point-on-a-circles-circumference-from-the-radiu  xP2=xP1+rsinθ; yP2=yP1−r(1−cosθ)

            Vector3 newWheelPos = new Vector3(newWheelX, movingWheel.y, newWheelZ);
            Vector3 newChairPos = (newWheelPos + pivotPos) / 2.0f;
            newChairPos.y = transform.position.y;

            Debug.Log(
                "pivot Wheel Pos:" +
                "\nx: " + pivotPos.x +
                "\ny: " + pivotPos.y +
                "\nz: " + pivotPos.z +
                "\nnew Wheel Pos:" +
                "\nx: " + newWheelPos.x +
                "\ny: " + newWheelPos.y +
                "\nz: " + newWheelPos.z +
                "\nnew Chair Pos:" +
                "\nx: " + newChairPos.x +
                "\ny: " + newChairPos.y +
                "\nz: " + newChairPos.z
            );

            chair.transform.position = newChairPos;
            RotateChair(angle);
            */
        }/*
        else if (d0 < 0 && d1 < 0) { // Both wheels moving backwards

        } else if (d0 >= 0 && d1 < 0) {  // Left wheel moving forwards, right moving backwards

        } else { // Left wheel moving backwards, right moving forwards

        }*/

        //chair.transform.position += chair.transform.forward * moveDelta[0];
    }

    private void MoveChair(float distance) {
        chair.transform.position += chair.transform.forward * distance;
        Debug.Log("Moving chair " + distance);
    }

    private void RotateChair(float angle) {
        Vector3 eulers = new Vector3(0.0f, angle, 0.0f);
        chair.transform.Rotate(eulers);
    }
}
