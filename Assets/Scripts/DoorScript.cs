using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour, Triggerable
{
    // Start is called before the first frame update

    private bool isOpen = false;
    private float closedPos, openPos;
    public float Speed;
    public AudioClip openClip, closeClip;
    public AudioSource speaker;

    public bool GetOpen() {
        return isOpen;
    }

    void Start()
    {
        closedPos = transform.position.y;
        openPos = closedPos + 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen) {
            if(transform.position.y < openPos) {
                transform.position += new Vector3(0, Speed * Time.deltaTime, 0);
                if(transform.position.y > openPos) {
                    transform.position = new Vector3(transform.position.x, openPos, transform.position.z);
                    //Close();
                }
            }
        }
        if (!isOpen) {
            if (transform.position.y > closedPos) {
                transform.position -= new Vector3(0, Speed * Time.deltaTime, 0);
                if (transform.position.y < closedPos) {
                    transform.position = new Vector3(transform.position.x, closedPos, transform.position.z);
                }
            }
        }
    }

    public void Trigger() {
        Close();
    }

    public void Open() {
        if(!isOpen){
            isOpen = true;
            speaker.PlayOneShot(openClip, 0.7f);
        }
    }

    public void Close() {
        if(isOpen){
            isOpen = false;
            speaker.PlayOneShot(closeClip, 0.7f);
        }
    }
}
