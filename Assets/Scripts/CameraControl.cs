using UnityEngine;

public class CameraControl : MonoBehaviour {

    // ** Static camera follow variables ** //

    [SerializeField] private int cameraOffSet;

    //Camera follow experimental variables
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private Vector3 cameraOffset3D;
    [SerializeField] private float cameraSmoothSpeed = 0.125f;

    private Vector3 desiredCameraPosition;
    private Vector3 smoothedCameraPosition;

    // ** Camera boundary follow variables ** //

    private BoxCollider2D cameraBox;
    private Transform player;
    private BoxCollider2D areaBoundaries;

    private Camera _camera;
    private float xRatio;
    private float yRatio;
    private float boxSizeX;
    private float boxSizeY;
    
    private void Start() {

        cameraBox = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        areaBoundaries = GameObject.FindGameObjectWithTag("Boundary").GetComponent<BoxCollider2D>();
        _camera = GetComponent<Camera>();

    }

    private void FixedUpdate() {

        //cameraBoundariesFollow();

    }

    // *** Camera boundary follow methods *** //

    // !!! REMOVED !! //

    // *** Static camera follow methods *** //

    private void cameraFollow() {

        // ** No smoothing ** //

        transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y, -cameraOffSet);

    }

    //Not working correctly
    private void cameraFollow_Experimental() {

        // ** With smoothing ** //

        desiredCameraPosition = targetToFollow.position + cameraOffset3D;
        smoothedCameraPosition = Vector3.Lerp(transform.position, desiredCameraPosition, cameraSmoothSpeed * Time.deltaTime);

        transform.position = smoothedCameraPosition;

    }

}
