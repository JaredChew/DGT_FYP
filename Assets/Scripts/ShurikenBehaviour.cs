using UnityEngine;

public class ShurikenBehaviour : MonoBehaviour {

    private Rigidbody2D shurikenRigidBody;

    private float throwSpeed = 5f;

    private bool isThrowing = false;

    // Use this for initialization
    void Start () {

        shurikenRigidBody = GetComponent<Rigidbody2D>();
            
    }
	
	// Update is called once per frame
	void Update () {

        if (isThrowing) {
            shurikenRigidBody.velocity = new Vector2(throwSpeed, shurikenRigidBody.velocity.y);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        switch (collision.gameObject.layer) {

            case 4: //Water
                resetThrowSpeedDirection();
                gameObject.SetActive(false);
                Destroy(gameObject);
                break;

            case 9: //Ground
                resetThrowSpeedDirection();
                gameObject.SetActive(false);
                Destroy(gameObject);
                break;

            case 10: //Fall off
                resetThrowSpeedDirection();
                gameObject.SetActive(false);
                Destroy(gameObject);
                break;

        }

        if (collision.gameObject.CompareTag(Global.gameObjectTag_Player)) {
            resetThrowSpeedDirection();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag(Global.gameObjectTag_PlayerSmoked)) {
            resetThrowSpeedDirection();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

    }

    private void resetThrowSpeedDirection() {
        
        if (throwSpeed < 0) {
            throwSpeed = -throwSpeed;
        }

        isThrowing = false;

    }

    public void throwShuriken(float x, float y, float offSet, bool throwRight) {
        
        if(!throwRight) {
            throwSpeed = -throwSpeed;
            transform.position = new Vector2(x - offSet, y);
        }
        else {
            transform.position = new Vector2(x + offSet, y);
        }

        isThrowing = true;

    }

}
