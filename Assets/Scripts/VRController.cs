using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour
{
    public GameObject chairRig;

    // Start is called before the first frame update
    void Start()
    {
        centerHeadset();
    }

    void centerHeadset() {
        GameObject cameraRig = GameObject.Find("[CameraRig]");
        GameObject headset = GameObject.Find("Camera");
        Vector3 headsetPos = headset.transform.localPosition;
        Vector3 newPos = chairRig.transform.position - headsetPos;
        newPos.y += 0.7f;
        cameraRig.transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetAxis("Fire1") > 0) {
            centerHeadset();
        }
    }
}
