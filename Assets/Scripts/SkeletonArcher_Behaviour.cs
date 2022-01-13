using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcher_Behaviour : MonoBehaviour {

    [SerializeField] private LayerMask wallObjects;
    [SerializeField] private LayerMask playerObject;
    [SerializeField] private LayerMask enemyObjects;

    [SerializeField] private GameObject arrowObject;
    [SerializeField] private GameObject particle;

    [SerializeField] private bool debug = false;
    [SerializeField] private bool lookingRight = true;

    [SerializeField] private float health = 1;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float wallVisionDistance = 1f;
    [SerializeField] private float playerVisionDistance = 10f;
    [SerializeField] private float attackVisionDistance = 5f;
    [SerializeField] private float attackRecoveryDuration = 1f;
    [SerializeField] private float charmRecoveryDuration = 4f;
    
    private Transform eyes;
    private Animator enemyAnimator;
    private Rigidbody2D enemyRigidbody;

    private bool isDead;
    private bool playerDetected;

    private float attackRecoveryCountdown;
    private float realityRecoveryCountdown;

    private void Awake() {

        isDead = false;
        playerDetected = false;

        attackRecoveryCountdown = 0f;
        realityRecoveryCountdown = 0f;

        enemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        eyes = transform.Find("Eyes").GetComponent<Transform>();

    }

    // Use this for initialization
    void Start() {

        if (lookingRight) {
            //Flip to left
            enemyRigidbody.transform.localScale = new Vector3(Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }
        else if (!lookingRight) {
            //Flip to right
            enemyRigidbody.transform.localScale = new Vector3(-Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }

    }

    private void Update() {

        if (health == 0 && !isDead) {
            isDead = true;
            //Change
            //enemyAnimator.SetBool(Global.playerAnimatorVariable_Death, true);
            enemyRigidbody.velocity = Vector2.zero;
            enemyAnimator.SetTrigger(Global.enemyAnimatorVariable_Death);
        }

        enemyAnimator.SetFloat(Global.enemyAnimatorVariable_Velocity, enemyRigidbody.velocity.magnitude);

    }

    // Update is called once per frame
    void FixedUpdate() {

        if (attackRecoveryCountdown <= 0 && !playerDetected) {
            movement();
        }
        else if (attackRecoveryCountdown <= 0 && playerDetected) {
            enemyRigidbody.velocity = Vector2.zero;
        }

        //Flip enemy if close to wall
        if (lookingRight && Physics2D.Raycast(eyes.position, Vector2.right, wallVisionDistance, wallObjects)) {
            flipEnemy();
        }
        else if (!lookingRight && Physics2D.Raycast(eyes.position, Vector2.left, wallVisionDistance, wallObjects)) {
            flipEnemy();
        }

        //Flip when enemy sees other enemies
        if (lookingRight && Physics2D.Raycast(eyes.position, Vector2.right, wallVisionDistance, enemyObjects)) {
            flipEnemy();
        }
        else if (!lookingRight && Physics2D.Raycast(eyes.position, Vector2.left, wallVisionDistance, enemyObjects)) {
            flipEnemy();
        }

        //Check if enemy sees player
        if (lookingRight) {
            playerDetected = Physics2D.Raycast(eyes.position, Vector2.right, playerVisionDistance, playerObject);
        }
        else {
            playerDetected = Physics2D.Raycast(eyes.position, Vector2.left, playerVisionDistance, playerObject);
        }

        if (playerDetected && attackRecoveryCountdown <= 0) {

            //Change
            realityRecoveryCountdown = charmRecoveryDuration;

            if (Physics2D.Raycast(eyes.position, Vector2.right, attackVisionDistance, playerObject) || Physics2D.Raycast(eyes.position, Vector2.left, attackVisionDistance, playerObject)) {
                enemyAnimator.SetBool(Global.enemyAnimatorVariable_Attack, true);
                attackRecoveryCountdown = attackRecoveryDuration;
                enemyRigidbody.velocity = Vector2.zero;
                // For make effect - now no need
                /*
                if (lookingRight)
                {
                    particle.GetComponent<Transform>().localScale = new Vector3(particle.GetComponent<Transform>().localScale.x, particle.GetComponent<Transform>().localScale.y, particle.GetComponent<Transform>().localScale.z);
                    Instantiate(particle, eyes.position, Quaternion.identity);
                }
                if (lookingRight)
                {
                    particle.GetComponent<Transform>().localScale = new Vector3(-particle.GetComponent<Transform>().localScale.x, particle.GetComponent<Transform>().localScale.y, particle.GetComponent<Transform>().localScale.z);
                    Instantiate(particle, eyes.position, Quaternion.identity);
                }
                */
            }

        }

        if (realityRecoveryCountdown > 0 && attackRecoveryCountdown <= 0) {
            //Change
            //Flip enemy after having vision on player to check if player is behind
            if (lookingRight && Physics2D.Raycast(eyes.position, Vector2.left, playerVisionDistance, playerObject)) {
                flipEnemy();
            }
            else if (!lookingRight && Physics2D.Raycast(eyes.position, Vector2.right, playerVisionDistance, playerObject)) {
                flipEnemy();
            }

        }

        if (realityRecoveryCountdown > 0) {
            realityRecoveryCountdown -= Time.deltaTime;
        }

        if (attackRecoveryCountdown > 0) {
            attackRecoveryCountdown -= Time.deltaTime;
        }

        // ** DEBUG ** //
        if (debug) { debugVision(); }

    }

    private void shotArrow()
    {

        ArrowBehaviour arrowObjectManipulate = (Instantiate(arrowObject)).GetComponent<ArrowBehaviour>();
        arrowObjectManipulate.shootArrow(eyes.position.x, eyes.position.y, 1, lookingRight);

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag(Global.gameObjectTag_Weapon)) {
            health--;
        }

    }

    /*
    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.collider.CompareTag(Global.gameObjectTag_Weapon)) {
            health--;
        }

    }
    */
    private void movement() {

        if (lookingRight) {
            enemyRigidbody.velocity = new Vector2(movementSpeed, enemyRigidbody.velocity.y);
        }
        else {
            enemyRigidbody.velocity = new Vector2(-movementSpeed, enemyRigidbody.velocity.y);
        }

        enemyAnimator.SetFloat(Global.enemyAnimatorVariable_Velocity, enemyRigidbody.velocity.x);

    }

    private void flipEnemy() {

        // Change
        if (lookingRight && enemyRigidbody.transform.localScale.x > 0) {
            //Flip to left
            lookingRight = false;
            enemyRigidbody.transform.localScale = new Vector3(-Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }
        // Change
        else if (!lookingRight && enemyRigidbody.transform.localScale.x < 0) {
            //Flip to right
            lookingRight = true;
            enemyRigidbody.transform.localScale = new Vector3(Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }

    }

    private void setAnimatorAttackFlase() {
        enemyAnimator.SetBool(Global.enemyAnimatorVariable_Attack, false);
    }

    private void destroyObject() {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void spawnAt(float x, float y, bool lookingRight) {

        transform.position = new Vector2(x, y);

        if (this.lookingRight != lookingRight) { flipEnemy(); }

    }

    private void debugVision() {

        if (lookingRight) {
            Debug.DrawRay(eyes.position, Vector2.right * playerVisionDistance, Color.green);
        }
        else {
            Debug.DrawRay(eyes.position, Vector2.left * playerVisionDistance, Color.green);
        }

    }

}