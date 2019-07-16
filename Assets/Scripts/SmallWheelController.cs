using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallWheelController : MonoBehaviour
{
    public BigWheelController bigWheel;
    public SeatController chairRig;
    private float wheelHeight, circumference;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer wheelModel = GetComponent<MeshRenderer>();

        wheelHeight = wheelModel.bounds.size.y;
        //Debug.Log("Wheel height is " + wheelHeight);
        float radius = wheelHeight / 2.0f;
        circumference = (2.0f * Mathf.PI * radius);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void turnWheel(float distance) {
        float degrees = (distance / circumference) * 360.0f;
        //Debug.Log("Smallwheel stats: " + distance + " " + degrees + " " + circumference);
        transform.Rotate(degrees, 0.0f, 0.0f);
    }
}
