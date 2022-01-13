using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Ghost_Behaviour : MonoBehaviour {

    [SerializeField] private LayerMask wallObjects;
    [SerializeField] private LayerMask playerObject;
    [SerializeField] private LayerMask enemyObjects;

    [SerializeField] private bool debug = false;
    [SerializeField] private bool lookingRight = true;

    [SerializeField] private float health = 1;
    [SerializeField] private float pushForce = 100f;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float wallVisionDistance = 1f;
    [SerializeField] private float playerVisionDistance = 4f;
    [SerializeField] private float attackVisionDistance = 3f;
    [SerializeField] private float attackRecoveryDuration = 0.75f;
    [SerializeField] private float charmRecoveryDuration = 4f;

    //private enum direction { top, right, left };

    private Transform eyes;
    private Transform weapon;
    private Animator enemyAnimator;
    private Rigidbody2D enemyRigidbody;
    private AttackManager attackCollisionCheck;
    private Transform tempPlayerPosition;

    private bool isDead = false;
    private bool isAttack = false;
    //private bool deathAttack = false;
    private bool playerDetected = false;

    private float attackRecoveryCountdown;
    private float realityRecoveryCountdown;

    private void Awake() {

        isDead = false;
        isAttack = false;
        playerDetected = false;

        attackRecoveryCountdown = 0f;
        realityRecoveryCountdown = 0f;

        enemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        weapon = transform.Find("Weapon").GetComponent<Transform>();
        eyes = transform.Find("Eyes").GetComponent<Transform>();
        attackCollisionCheck = transform.Find("Weapon").GetComponent<AttackManager>();
        //tempPlayerPosition = new Vector3(0, 0, 0);

        weapon.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start() {

        if (lookingRight) {
            //Flip to left
            enemyRigidbody.transform.localScale = new Vector3(-Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }
        else if (!lookingRight) {
            //Flip to right
            enemyRigidbody.transform.localScale = new Vector3(Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }

    }

    private void Update() {

        if (health == 0 && !isDead) {
            isDead = true;
            // Changed
            //destroyObject(); //Bat has no death animation
            enemyRigidbody.velocity = Vector2.zero;
            enemyAnimator.SetTrigger(Global.enemyAnimatorVariable_Death);
        }

        if (attackCollisionCheck.getEnemyParrying()) {
            attackRecoveryCountdown = attackRecoveryDuration;
        }

        if (attackCollisionCheck.getWeaponHit()) {

            if (lookingRight) { enemyRigidbody.AddForce(new Vector2((int)(-pushForce), 0)); }
            else if (!lookingRight) { enemyRigidbody.AddForce(new Vector2((int)(pushForce), 0)); }

            attackCollisionCheck.setWeaponHit(false);

        }

        enemyAnimator.SetFloat(Global.enemyAnimatorVariable_Velocity, enemyRigidbody.velocity.magnitude);

    }

    // Update is called once per frame
    void FixedUpdate() {

        if (attackRecoveryCountdown <= 0 && !playerDetected) {
            movement();
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

            realityRecoveryCountdown = charmRecoveryDuration;

            if ( !isAttack && ( Physics2D.Raycast(eyes.position, Vector2.right, attackVisionDistance, playerObject) || Physics2D.Raycast(eyes.position, Vector2.left, attackVisionDistance, playerObject) ) ) {

                enemyAnimator.SetTrigger(Global.enemyAnimatorVariable_Aimming);

                RaycastHit2D hit = Physics2D.Raycast(eyes.position, lookingRight ? Vector2.right : Vector2.left, attackVisionDistance, playerObject);
                tempPlayerPosition = hit.rigidbody.GetComponent<Transform>();

                attackRecoveryCountdown = attackRecoveryDuration;
                isAttack = true;
                enemyRigidbody.velocity = Vector2.zero;

            }

        }

        if (realityRecoveryCountdown > 0 && attackRecoveryCountdown <= 0 && !isAttack) {
            //Change
            //Flip enemy after having vision on player to check if player is behind
            if (lookingRight && Physics2D.Raycast(eyes.position, Vector2.left, playerVisionDistance, playerObject)) {
                flipEnemy();
            }
            else if (!lookingRight && Physics2D.Raycast(eyes.position, Vector2.right, playerVisionDistance, playerObject)) {
                flipEnemy();
            }

        }
        if (realityRecoveryCountdown > 0 && !isAttack) {
            realityRecoveryCountdown -= Time.deltaTime;
        }

        if (attackRecoveryCountdown > 0 && !isAttack) {
            attackRecoveryCountdown -= Time.deltaTime;
        }
        /*
        if (isAttack && attackRecoveryCountdown <= 0) {
            attackRecoveryCountdown = attackRecoveryDuration;
        }
        */
        // ** DEBUG ** //
        if(debug) { debugVision(); }

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (!attackCollisionCheck.getWeaponHit() && collision.CompareTag(Global.gameObjectTag_Weapon)) {
            health--; //health = 0;
        }

    }
    /*
    private void OnCollisionEnter2D(Collision2D collision) {

        if (!attackCollisionCheck.getWeaponHit() && collision.collider.CompareTag(Global.gameObjectTag_Weapon)) {
            health--; //health = 0;
        }
        
        else if (collision.collider.CompareTag(Global.gameObjectTag_Weapon) && !deathAttack) {

            enemyRigidbody.velocity = Vector2.zero;

            switch (UnityEngine.Random.Range(0, Enum.GetNames(typeof(direction)).Length)) {

                case (int)direction.top:
                    //transform.position = new Vector2(transform.position.x, spGameManager.getPlayerPositionY() - 1);
                    enemyRigidbody.AddForce(new Vector2(0, pushForce));
                    break;

                case (int)direction.right:
                    //transform.position = new Vector2(spGameManager.getPlayerPositionX() + 1, transform.position.y);
                    enemyRigidbody.AddForce(new Vector2(pushForce, 0));
                    break;

                case (int)direction.left:
                    //transform.position = new Vector2(spGameManager.getPlayerPositionX() - 1, transform.position.y);
                    enemyRigidbody.AddForce(new Vector2(-pushForce, 0));
                    break;

            }
            
            deathAttack = true;

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

        // Changed
        if (lookingRight && enemyRigidbody.transform.localScale.x < 0) {
            //Flip to left
            lookingRight = false;
            enemyRigidbody.transform.localScale = new Vector3(Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }
        // Changed
        else if (!lookingRight && enemyRigidbody.transform.localScale.x > 0) {
            //Flip to right
            lookingRight = true;
            enemyRigidbody.transform.localScale = new Vector3(-Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }

    }

    private void attackPlayer() {

        if ( tempPlayerPosition.GetComponent<Player>().getLookingRight() ) {
            spawnAt(tempPlayerPosition.GetComponent<Rigidbody2D>().velocity.x == 0f ? tempPlayerPosition.position.x : tempPlayerPosition.position.x + 1f, tempPlayerPosition.position.y, lookingRight);
        }
        else {
            spawnAt(tempPlayerPosition.GetComponent<Rigidbody2D>().velocity.x == 0f ? tempPlayerPosition.position.x : tempPlayerPosition.position.x - 1f, tempPlayerPosition.position.y, lookingRight);
        }
        
        enemyAnimator.SetBool(Global.enemyAnimatorVariable_Attack, true);
    }

    private void setAnimatorAttackFlase() {
        enemyAnimator.SetBool(Global.enemyAnimatorVariable_Attack, false);
        isAttack = false;
    }

    private void setWeaponActiveFalse() {
        weapon.gameObject.SetActive(false);
    }

    private void setWeaponActiveTrue() {
        weapon.gameObject.SetActive(true);
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
            Debug.DrawRay(eyes.position, Vector2.right * attackVisionDistance, Color.red);
        }
        else {
            Debug.DrawRay(eyes.position, Vector2.left * playerVisionDistance, Color.green);
            Debug.DrawRay(eyes.position, Vector2.left * attackVisionDistance, Color.red);
        }

    }

}