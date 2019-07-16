using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonScript : MonoBehaviour, Triggerable
{
    // Start is called before the first frame update
    public DoorScript door;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Trigger() {
        if(door.GetOpen()){
            door.Close();
        } else {
            door.Open();
        }
    }
}
