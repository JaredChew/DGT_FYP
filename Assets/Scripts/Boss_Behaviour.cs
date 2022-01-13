using UnityEngine;

using System;

public class Boss_Behaviour : MonoBehaviour {

    [SerializeField] private SingleplayerGameManager spGameManager;

    [SerializeField] private LayerMask playerObject;
    [SerializeField] private LayerMask groundObjects;

    [SerializeField] private bool lookingRight = true;

    [SerializeField] private float health = 10;
    [SerializeField] private float pushForce = 500f;
    [SerializeField] private float movementSpeed = 2f;

    [SerializeField] private int maxAttack_Phase2 = 4;

    private Transform groundCheck;
    private Animator enemyAnimator;
    private Rigidbody2D enemyRigidbody;
    private AttackManager attackCollisionCheck;
    
    private bool onGround;
    private bool pauseBehaviour;

    private float groundCheckRadius = 0.1f;

    private float throwItemsCounter_Phase1;
    private float positionStayCounter_Phase1;
    private float teleportTotal_Phase2;

    private int attackCount_Phase2;

    private bool attack_Phase3;
    private float pauseCounter_Phase3;

    private bool teleportMiddle_Pahse4;

    private void Awake() {
        
        onGround = false;
        pauseBehaviour = false;

        groundCheckRadius = 0.1f;

        enemyAnimator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();

        groundCheck = transform.Find("Ground Check").GetComponent<Transform>();
        attackCollisionCheck = transform.Find("Weapon").GetComponent<AttackManager>();

        throwItemsCounter_Phase1 = UnityEngine.Random.Range(3, 5);
        positionStayCounter_Phase1 = UnityEngine.Random.Range(0, 15);

        attackCount_Phase2 = maxAttack_Phase2;
        teleportTotal_Phase2 = UnityEngine.Random.Range(1, 5);

        attack_Phase3 = false;
        pauseCounter_Phase3 = UnityEngine.Random.Range(5, 7);

        teleportMiddle_Pahse4 = false;

    }

    // Use this for initialization
    void Start() {

        if (!lookingRight) {
            //Flip to left
            lookingRight = false;
            enemyRigidbody.transform.localScale = new Vector3(-Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }

    }

    private void Update() {

        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundObjects);

        if(onGround) {

            enemyAnimator.SetFloat(Global.bossAnimatorVariable_Velocity, enemyRigidbody.velocity.magnitude);

            if (enemyAnimator.GetBool(Global.bossAnimatorVariable_DashAttack) && enemyAnimator.GetFloat(Global.bossAnimatorVariable_Velocity) < 0.1) {
                //enemyAnimator.SetBool(Global.bossAnimatorVariable_DashAttack, false);
            }

            enemyAnimator.SetBool(Global.bossAnimatorVariable_OnGround, true);
        }

        if (attackCollisionCheck.getWeaponHit()) {

            if (lookingRight) { enemyRigidbody.AddForce(new Vector2((int)(-pushForce * 0.65), 0)); }
            else if (!lookingRight) { enemyRigidbody.AddForce(new Vector2((int)(pushForce * 0.65), 0)); }

            attackCollisionCheck.setWeaponHit(false);

        }
        else if (attackCollisionCheck.getEnemyParrying()) {
            
            pauseBehaviour = true;

            attackCollisionCheck.setEnemyParrying(false);

            enemyAnimator.SetBool(Global.bossAnimatorVariable_Parried, true);

        }

    }

    // Update is called once per frame
    void FixedUpdate() {

        //UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        if (spGameManager.playBossFight() && !pauseBehaviour) {

            switch ((int)health) {

                case 0:
                    enemyAnimator.SetBool(Global.bossAnimatorVariable_Kneeling, false);
                    enemyAnimator.SetBool(Global.bossAnimatorVariable_Death, true);
                    spGameManager.nextScenario();
                    break;

                case 1:
                    phase4();
                    break;

                case 2:
                case 3:
                case 4:
                    phase3();
                    break;

                case 5:
                case 6:
                case 7:
                    phase2();
                    break;

                case 8:
                case 9:
                case 10:
                    phase1();
                    break;

            }

        }

    }

    private void phase1() {

        switch ((int)positionStayCounter_Phase1) {

            case 0:

                switch (UnityEngine.Random.Range(0, 2)) {

                    case 0:
                        if (!lookingRight) { flipEnemy(); }
                        transform.position = new Vector2(spGameManager.getBossPhase1TeleportA_X(), spGameManager.getBossPhase1TeleportA_Y());
                        break;

                    case 1:
                        if (lookingRight) { flipEnemy(); }
                        transform.position = new Vector2(spGameManager.getBossPhase1TeleportB_X(), spGameManager.getBossPhase1TeleportB_Y());
                        break;

                }

                positionStayCounter_Phase1 = UnityEngine.Random.Range(0, 6);

                break;

            default:

                switch((int)throwItemsCounter_Phase1) {

                    case 0:
                        Invoke("useItem", 0);
                        throwItemsCounter_Phase1 = UnityEngine.Random.Range(2, 5);
                        break;

                    default:
                        throwItemsCounter_Phase1 -= Time.deltaTime;
                        positionStayCounter_Phase1 -= Time.deltaTime;
                        break;

                }

                break;


        }

    }

    private void phase2() {

        if (teleportTotal_Phase2 <= 0 && !enemyAnimator.GetBool(Global.bossAnimatorVariable_Attack)) {

            switch (UnityEngine.Random.Range(0, 2)) {

                case 0:
                    if (!lookingRight) { flipEnemy(); }
                    transform.position = new Vector2(spGameManager.getPlayerPositionX() - 2, spGameManager.getPlayerPositionY());
                    break;

                case 1:
                    if (lookingRight) { flipEnemy(); }
                    transform.position = new Vector2(spGameManager.getPlayerPositionX() + 2, spGameManager.getPlayerPositionY());
                    break;

            }

            attackCount_Phase2--;
            enemyAnimator.SetBool(Global.enemyAnimatorVariable_Attack, true);

            if (attackCount_Phase2 == 1) {
                //heavy attack
            }
            else if (attackCount_Phase2 == 0) {
                teleportTotal_Phase2 = UnityEngine.Random.Range(2, 6);
                attackCount_Phase2 = maxAttack_Phase2;
            }

        }
        else if (teleportTotal_Phase2 <= 0) {
            teleportTotal_Phase2 = UnityEngine.Random.Range(2, 6);
        }

        teleportTotal_Phase2 -= Time.deltaTime;

    }

    private void phase3() {

        if (!attack_Phase3 && pauseCounter_Phase3 <= 0) {

            enemyAnimator.SetBool(Global.bossAnimatorVariable_DashAttack, false);

            switch (UnityEngine.Random.Range(0, 2)) {

                case 0:
                    if (!lookingRight) { flipEnemy(); }
                    transform.position = new Vector2(spGameManager.getPlayerPositionX() - 1, spGameManager.getPlayerPositionY());
                    enemyRigidbody.AddForce(new Vector2(-5000, 3000));
                    break;

                case 1:
                    if (lookingRight) { flipEnemy(); }
                    transform.position = new Vector2(spGameManager.getPlayerPositionX() + 1, spGameManager.getPlayerPositionY());
                    enemyRigidbody.AddForce(new Vector2(5000, 3000));
                    break;

            }

            attack_Phase3 = true; 

        }
        
        if (onGround && attack_Phase3) {

            enemyAnimator.SetBool(Global.bossAnimatorVariable_DashAttack, true);

            if (lookingRight) {
                enemyRigidbody.AddForce(new Vector2(5000, 0));
            }
            else if (!lookingRight) {
                enemyRigidbody.AddForce(new Vector2(5000, 0));
            }

            pauseCounter_Phase3 = UnityEngine.Random.Range(2,5);

            attack_Phase3 = false;

        }
        
        if (pauseCounter_Phase3 > 0) {
            pauseCounter_Phase3 -= Time.deltaTime;
        }

    }

    private void phase4() {

        if (!teleportMiddle_Pahse4) {
            transform.position = new Vector2(spGameManager.getBossAreaMiddlePosX(), spGameManager.getBossAreaMiddlePosY());
            enemyAnimator.SetBool(Global.bossAnimatorVariable_Kneeling, true);
            teleportMiddle_Pahse4 = true;
        }

    }

    private void useItem() {

        switch(UnityEngine.Random.Range(0, Enum.GetNames(typeof(Global.bossItem)).Length)) {

            case (int)Global.bossItem.fireBomb:

                fireBombBehaviour fireBombObjectManipulate = (Instantiate(spGameManager.getBossItem(Global.bossItem.fireBomb))).GetComponent<fireBombBehaviour>();

                fireBombObjectManipulate.throwFireBomb(transform.position.x, transform.position.y, 1, lookingRight);

                break;

            case (int)Global.bossItem.kunai:

                KunaiBehaviour kunaiObjectManipulate = (Instantiate(spGameManager.getBossItem(Global.bossItem.kunai))).GetComponent<KunaiBehaviour>();

                kunaiObjectManipulate.throwKunai(transform.position.x, transform.position.y, 1, lookingRight);

                break;

            case (int)Global.bossItem.shuriken:

                ShurikenBehaviour shurikenObjectManipulate = (Instantiate(spGameManager.getBossItem(Global.bossItem.shuriken))).GetComponent<ShurikenBehaviour>();

                shurikenObjectManipulate.throwShuriken(transform.position.x, transform.position.y, 1, lookingRight);

                break;

        }

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.collider.CompareTag(Global.gameObjectTag_Weapon)) {
            health--;
        }

    }
    /*
    private void movement() {

        if (lookingRight) {
            enemyRigidbody.velocity = new Vector2(movementSpeed, enemyRigidbody.velocity.y);
        }
        else {
            enemyRigidbody.velocity = new Vector2(-movementSpeed, enemyRigidbody.velocity.y);
        }

        enemyAnimator.SetFloat(Global.enemyAnimatorVariable_Velocity, enemyRigidbody.velocity.x);

    }
    */
    private void flipEnemy() {

        if (!lookingRight) {
            //Flip to right
            lookingRight = true;
            enemyRigidbody.transform.localScale = new Vector3(Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }
        else if (lookingRight) {
            //Flip to left
            lookingRight = false;
            enemyRigidbody.transform.localScale = new Vector3(-Mathf.Abs(enemyRigidbody.transform.localScale.x), enemyRigidbody.transform.localScale.y, enemyRigidbody.transform.localScale.z);
        }

    }

    private void SetAttackAnimationFlase() {
        enemyAnimator.SetBool(Global.bossAnimatorVariable_Attack, false);
        pauseBehaviour = false;
    }

    private void setParriedAnimationFalse() {
        enemyAnimator.SetBool(Global.bossAnimatorVariable_Parried, false);
        pauseBehaviour = false;
    }

    private void destroyObject() {
        pauseBehaviour = true;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void spawnAt(float x, float y, bool lookingRight) {

        transform.position = new Vector2(x, y);

        if (this.lookingRight != lookingRight) { flipEnemy(); }

    }

}