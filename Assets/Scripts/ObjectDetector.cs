using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour {

    private bool playerDetected = false;
    private bool platformDetected = false;

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.CompareTag(Global.gameObjectTag_Player)) {
            playerDetected = true;
        }

        if (collision.gameObject.CompareTag(Global.gameObjectTag_Platform)) {
            platformDetected = true;
        }

    }

    public bool getPlayerDetected() {

        if (playerDetected) {
            playerDetected = false;
            return true;
        }

        return false;

    }

    public bool getPlatformDetected() {

        if (platformDetected) {
            platformDetected = false;
            return true;
        }

        return false;

    }

}
