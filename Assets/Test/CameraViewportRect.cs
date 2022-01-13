using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewportRect : MonoBehaviour {

    public float xPos, yPos, width, height;

    // Use this for initialization
    void Start () {

        xPos = 0;
        yPos = 0;
        width = 1;
        height = 1;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        gameObject.GetComponent<Camera>().pixelRect = new Rect(Screen.width - xPos, Screen.height - yPos, width, height);
    }

}
