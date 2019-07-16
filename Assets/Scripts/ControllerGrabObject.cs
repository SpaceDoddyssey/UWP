using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerGrabObject : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabGripAction;
    public SteamVR_Action_Boolean triggerAction;
    private GameObject collidingObject, objectInHand;
    private BigWheelController wheelCon;
    public GameObject chairRig;

    float lastAngle = 0;
    bool isGrabbingWheel = false;
    bool isBraking = false;

    private void SetCollidingObject(Collider col) {
        // 1
        if (collidingObject || !col.GetComponent<Rigidbody>()) {
            return;
        }
        // 2
        collidingObject = col.gameObject;
    }
    public void OnTriggerEnter(Collider other) {
        SetCollidingObject(other);
    }
    // 2
    public void OnTriggerStay(Collider other) {
        SetCollidingObject(other);
    }
    // 3
    public void OnTriggerExit(Collider other) {
        if (!collidingObject) {
            return;
        }

        if(objectInHand) {
            ReleaseObject();
        }
        collidingObject = null;
    }


    private void GrabObject(bool usedTrigger) {
        
        var scripts = collidingObject.GetComponents<Triggerable>();
        foreach(Triggerable script in scripts){
            Debug.Log("Found Script");
            script.Trigger();
        }

        objectInHand = collidingObject;
        //Debug.Log("Grabbing " + collidingObject.name);
        collidingObject = null;

        if(objectInHand.tag == "WheelZone") {
            if (objectInHand.name.Equals("wheelZoneL")) {
                wheelCon = GameObject.Find("wheelBigL").GetComponent<BigWheelController>();
            }
            else {
                wheelCon = GameObject.Find("wheelBigR").GetComponent<BigWheelController>();
            }

            if(!usedTrigger){
                isGrabbingWheel = true;
                lastAngle = GetConAngle(objectInHand);
            }

            if (usedTrigger) {
                isBraking = true;
            }
        }
    }

    float GetConAngle(GameObject wheel) {
        Vector3 conWPos = gameObject.transform.position;
        Vector3 ConRelativePosToChair = chairRig.transform.InverseTransformPoint(conWPos);
        Vector3 axleToCon = ConRelativePosToChair - wheel.transform.localPosition;
        axleToCon.Normalize();
        float angleInRads = Mathf.Atan2(axleToCon.y, axleToCon.z);
        float angleInDegs = angleInRads * Mathf.Rad2Deg;
        //Debug.Log("Angle = " + angleInDegs);
        return angleInDegs;
    }

    private void ReleaseObject() {
        //Debug.Log("Released " + objectInHand.name);
        objectInHand = null;
        isGrabbingWheel = false;
        isBraking = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (grabGripAction.GetLastStateDown(handType)) {
            //Debug.Log("Update grab " + handType);
            if (collidingObject) {
                GrabObject(false);
            }
        }

        if (grabGripAction.GetLastStateUp(handType)) {
            //Debug.Log("Update release" + handType);
            if (objectInHand) {
                ReleaseObject();
            }
        }

        if (triggerAction.GetLastStateDown(handType)) {
            if(collidingObject){
                GrabObject(true);
            }
        }

        if (triggerAction.GetLastStateUp(handType)) {
            //Debug.Log("Update release" + handType);
            if (objectInHand) {
                ReleaseObject();
            }
        }

        if (isGrabbingWheel) {
            float curAngle = GetConAngle(objectInHand);
            float deltaAngle = curAngle - lastAngle;
            lastAngle = curAngle;
            wheelCon.addForce(-deltaAngle);
            //Debug.Log("Delta angle = " + deltaAngle);
        }

        if (isBraking) {
            wheelCon.brake();
        }
    }
}
