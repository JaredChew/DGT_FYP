using UnityEngine;

public class BoundaryManager : MonoBehaviour {

    private BoxCollider2D managerBox;
    private Transform player;

    public GameObject boundary;

	// Use this for initialization
	void Start () {

        managerBox = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		
	}
	
	// Update is called once per frame
	void Update () {

        // !!! REMOVED !! //

	}

    void manageBoundary() {

        // !!! REMOVED !! //

    }

}
