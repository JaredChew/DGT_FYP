using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLight : MonoBehaviour {

    public Transform transformLight;
    public Transform transformPlayer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // vector2;
        //vector2 = new Vector2(gameObjectPlayer.transform.position.x, gameObjectPlayer.transform.position.y);
        transformLight.position = new Vector3(transformPlayer.position.x, transformPlayer.position.y, transformLight.position.z) ;

    }
}
