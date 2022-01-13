using UnityEngine;

public class GrapplingHookBehaviour : MonoBehaviour {

    [SerializeField] private float pullSpeed = 15f;
    [SerializeField] private float shootSpeed = 30f;

    private Rigidbody2D grapplingHookRigidBody;

    private bool isHooked = false;
    private bool isShooting = false;

    // Use this for initialization
    void Start() {

        grapplingHookRigidBody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update() {

        if (isShooting) {
            grapplingHookRigidBody.velocity = new Vector2(grapplingHookRigidBody.velocity.x, -shootSpeed);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        switch (collision.gameObject.layer) {

            case 4: //Water
                resetThrowSpeedDirection();
                isHooked = true;
                break;

            case 9: //Ground
                resetThrowSpeedDirection();
                isHooked = true;
                break;

            case 10: //Fall off
                resetThrowSpeedDirection();
                gameObject.SetActive(false);
                Destroy(gameObject);
                break;

        }

        if (collision.gameObject.CompareTag(Global.gameObjectTag_Player)) {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag(Global.gameObjectTag_PlayerSmoked)) {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

    }

    private void resetThrowSpeedDirection() {

        if (shootSpeed < 0) {
            shootSpeed = -shootSpeed;
        }

        isShooting = false;

    }

    public void shootGrapplingHook(float x, float y) {

        transform.position = new Vector2(x, y - (float)0.5);

        isShooting = true;

    }

    public float getPullSpeed() {

        return pullSpeed;

    }

    public float getPositionY() {

        return transform.position.y;

    }

    public bool getIsHooked() {

        return isHooked;

    }

    public void destroyObject() {
        
        gameObject.SetActive(false);
        Destroy(gameObject);

    }

}
