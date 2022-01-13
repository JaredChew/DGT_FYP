using UnityEngine;

public class ArrowBehaviour : MonoBehaviour {

    [SerializeField] private float shootSpeed = 50f;

    private Rigidbody2D arrowRigidBody;

    private bool isShooting = false;

    // Use this for initialization
    void Start () {

        arrowRigidBody = GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void Update () {

        if (isShooting) {
            arrowRigidBody.velocity = new Vector2(shootSpeed, arrowRigidBody.velocity.y);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        
        resetShootSpeedDirection();
        gameObject.SetActive(false);
        Destroy(gameObject);

    }

    private void resetShootSpeedDirection() {

        if (shootSpeed < 0) {
            shootSpeed = -shootSpeed;
        }

        isShooting = false;

    }

    public void shootArrow(float x, float y, float offSet, bool throwRight) {

        if (!throwRight) {
            shootSpeed = -shootSpeed;
            transform.position = new Vector2(x - offSet, y);
        }
        else {
            transform.position = new Vector2(x + offSet, y);
        }

        isShooting = true;

    }

}
