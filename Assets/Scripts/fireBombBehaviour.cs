using UnityEngine;

public class fireBombBehaviour : MonoBehaviour {

    private GameObject fireBomvVFX;
    //private Animator fireBombAnimator;
    private Rigidbody2D fireBombRigidBody;

    private float throwForce = 10f;

	// Use this for initialization
	void Start () {

        fireBombRigidBody = GetComponent<Rigidbody2D>();
        fireBomvVFX = transform.Find("VFX_Fire Background").GetComponent<Transform>().gameObject;

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        switch (collision.gameObject.layer) {

            case 4: //Water
                resetThrowForceDirection();
                gameObject.SetActive(false);
                Destroy(gameObject);
                break;

            case 9: //Ground
                resetThrowForceDirection();
                playFireBombHitAnimation();
                break;

            case 10: //Fall off
                resetThrowForceDirection();
                gameObject.SetActive(false);
                Destroy(gameObject);
                break;

        }

    }

    public void throwFireBomb(float x, float y, float offSet, bool throwRight) {


        if (!throwRight) {
            throwForce = -throwForce;
            transform.position = new Vector2(x - offSet, y);
        }
        else {
            transform.position = new Vector2(x + offSet, y);
        }

        fireBombRigidBody.AddForce(new Vector2(throwForce / 2, 0));

    }

    public void playFireBombHitAnimation() {

        //fireBombAnimator.SetTrigger(Global.itemAnimationVariable_FireBomb);
        fireBomvVFX.SetActive(true);

    }
    
    private void resetThrowForceDirection() {

        if (throwForce < 0) {
            throwForce = -throwForce;
        }

    }

    private void destroyObject() {
        
        gameObject.SetActive(false);
        Destroy(gameObject);

    }

}
