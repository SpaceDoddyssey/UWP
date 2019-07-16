using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandController : MonoBehaviour
{
    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/
    public SteamVR_Input_Sources handType; // 1
    public SteamVR_Action_Boolean grabAction; // 2

    // Update is called once per frame
    void Update()
    {
        if (GetGrab()) {
            print("Grab " + handType);
        }
    }

    public bool GetGrab() 
    {
        return grabAction.GetState(handType);
    }
}
