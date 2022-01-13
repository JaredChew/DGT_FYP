using UnityEngine;

public class MovingPlatformBehaviour : MonoBehaviour {

    [SerializeField] private ObjectDetector startPos;
    [SerializeField] private ObjectDetector endPos;

    [SerializeField] private float horizontalSpeed = 1f;
    [SerializeField] private float verticalSpeed = 1f;

    [SerializeField] private bool movingVertically = false;
    [SerializeField] private bool movingHorizontally = false;

    private Rigidbody2D platformRigidBody;

    private void Awake() {

        movingHorizontally = false;
        movingVertically = false;

        platformRigidBody = GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void Update () {

        if (endPos.getPlatformDetected() || startPos.getPlatformDetected()) {
            horizontalSpeed *= -1;
            verticalSpeed *= -1;
        }

    }

    private void FixedUpdate() {

        if (movingHorizontally) {
            platformRigidBody.velocity = new Vector2(platformRigidBody.velocity.x, verticalSpeed);
        }

        if (movingVertically) {
            platformRigidBody.velocity = new Vector2(horizontalSpeed, platformRigidBody.velocity.y);
        }

    }

}
