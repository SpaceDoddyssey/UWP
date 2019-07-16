using UnityEngine;
using System.Collections;

public class scr_camera : MonoBehaviour {

    public float rotate_amount;
    public float rotate_speed;
    public float angleOffset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float angle = Mathf.Sin(Time.realtimeSinceStartup * rotate_speed) * rotate_amount + angleOffset;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);
    }
}
