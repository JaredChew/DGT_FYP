using UnityEngine;

public class Player : MonoBehaviour {

    // ** Variables that are able to be tuned for better player control feel ** //

    // !! Values set here are its default values !! //

    //Object detection's layer variables
    [SerializeField] private LayerMask enemyObjects;
    [SerializeField] private LayerMask waterObjects;
    [SerializeField] private LayerMask groundObjects;
    [SerializeField] private LayerMask fallOffObjects;

    //Items's object
    [SerializeField] private GameObject kunaiObject;
    [SerializeField] private GameObject caltropObject;
    [SerializeField] private GameObject fireBombObject;
    [SerializeField] private GameObject shurikenObject;
    [SerializeField] private GameObject grapplingHookObject;

    //Player stats variables
    [SerializeField] private float health = 1f;
    [SerializeField] private float lockOnRange = 5f;
    [SerializeField] private float jumpForce = 1750f;
    [SerializeField] private float pushForce = 1000f;
    [SerializeField] private float movementSpeed = 6f;

    [SerializeField] private int maxStamina = 3;
    [SerializeField] private int playerNumber = 0;
    [SerializeField] private int maxPlayerJump = 1;

    [SerializeField] private bool lookingRight = true;
    [SerializeField] private bool takeDamageFromPlayer = true;

    // ** Variables that are able to be tuned for better player control feel only within code ** //

    //Surrounding checker size variables
    private const float wallCheckRadius = 0.7f;
    private const float groundCheckRadius = 0.1f;

    //Items variable
    private const float burningDuration = 5;
    private const float smokeBombRadius = 10f;
    private const float medicineDuration = 10f;
    private const float smokeBombDuration = 10f;
    private const float phaseWalkDuration = 10f;
    private const float caltroppedDuration = 4f;
    private const float ninjaSwordDuration = 10f;
    private const float footSpikesDuration = 10f;

    //Player recovery time variables
    private const float dodgeCooldownTime = 1f;
    private const float wallJumpCooldownTime = 3f;
    private const float staminaRecoveryDuration = 5f;
    private const float heavyAttackRecoveryDuration = 10f;

    // ** Variables used to control Player  ** //

    //Checker Object variables
    private Transform weapon;
    private Transform wallCheck;
    private Transform groundCheck;
    private Transform footSpikesUse;

    private AttackManager attackCollisionCheck;
    private RaycastHit2D lockOnPlayerDetection;

    //Player's main object variables
    private GameObject itemTracker;
    private Global.items itemAtHand;
    private Animator playerAnimator;
    private Rigidbody2D playerRigidBody;
    private PlayerDetailManager playerDetailManager;

    //Player VFX variables
    private GameObject smokeFX;
    private GameObject elixerFX;
    private GameObject portalsFX;

    //Current stats value variables
    private int currentJump;
    private int currentStamina;

    private float CurrentHeavyAttackCharge;

    //Counter variables
    private float burningCountdown;
    private float caltroppedCooldown;
    private float dodgeCooldownCounter;
    private float staminaRecoveryCounter;
    private float wallJumpCooldownCounter;
    private float itemUseCurrentDurationCounter;

    //Is Player on certain object checker variables
    private bool isOnWall;
    private bool isOnWater;
    private bool isOnGround;

    //Status checker variables
    private bool controlsLocked;
    private bool movementLocked;
    private bool staminaDepleted;
    private bool heavyAttackReady;

    //Action checker variables
    private bool isDead;
    private bool isOnFire;
    private bool isParried;
    private bool wasMoving;
    private bool isDefending;
    private bool isUsingItem;
    private bool isCaltropped;
    private bool isLockingOnTarget;
    private bool isAbleToPhaseWalk;

    // ***          FLOW AND UPDATE          *** //

    private void Awake() {

        // ** Use this for initialisation before Start() ** //

        isDead = false;
        isOnWall = false;
        isOnFire = false;
        isParried = false;
        wasMoving = false;
        isOnWater = false;
        isOnGround = false;
        isUsingItem = false;
        isDefending = false;
        isCaltropped = false;
        controlsLocked = false;
        movementLocked = false;
        staminaDepleted = false;
        heavyAttackReady = false;
        isLockingOnTarget = false;
        isAbleToPhaseWalk = false;

        burningCountdown = 0f;
        caltroppedCooldown = 0f;
        dodgeCooldownCounter = 0f;
        staminaRecoveryCounter = 0f;
        wallJumpCooldownCounter = 0f;
        CurrentHeavyAttackCharge = 0f;
        itemUseCurrentDurationCounter = 0f;

        currentJump = maxPlayerJump;
        currentStamina = maxStamina;

        itemAtHand = Global.items.itemNone;

    }

    void Start() {

        // ** Use this for initialization ** //

        playerAnimator = GetComponent<Animator>();

        playerRigidBody = GetComponent<Rigidbody2D>();


        weapon = transform.Find("Weapon").GetComponent<Transform>();
        wallCheck = transform.Find("Wall Check").GetComponent<Transform>();
        groundCheck = transform.Find("Ground Check").GetComponent<Transform>();
        footSpikesUse = transform.Find("Foot Spikes").GetComponent<Transform>();

        attackCollisionCheck = transform.Find("Weapon").GetComponent<AttackManager>();

        smokeFX = transform.Find("Smoke FX").GetComponent<Transform>().gameObject;
        portalsFX = transform.Find("Portals FX").GetComponent<Transform>().gameObject;
        elixerFX = transform.Find("VFX_Medicine").GetComponent<Transform>().gameObject;

        playerDetailManager = transform.Find("PlayerDetails_Canvas").GetComponent<PlayerDetailManager>();

        //Flips player on spawn if not looking right
        if (!lookingRight) {

            //Flip player's object
            playerRigidBody.transform.localScale = new Vector3(-Mathf.Abs(playerRigidBody.transform.localScale.x), playerRigidBody.transform.localScale.y, playerRigidBody.transform.localScale.z);

            //for flip back the PlayerDetail_Canvas
            transform.Find("PlayerDetails_Canvas").localScale = new Vector3(-Mathf.Abs(transform.Find("PlayerDetails_Canvas").localScale.x), transform.Find("PlayerDetails_Canvas").localScale.y, transform.Find("PlayerDetails_Canvas").localScale.z);

        }

        //Turn ff all VFX
        smokeFX.SetActive(false);
        elixerFX.SetActive(false);
        portalsFX.SetActive(false);

        //Hides attack check to be used only when using weapon
        weapon.gameObject.SetActive(false);

        //Hides foot spikes untill it is being used
        footSpikesUse.gameObject.SetActive(false);

        //For set animation of Stamina bar to default - ( full state )
        playerDetailManager.switchStaminaAnimation(currentStamina);

    }

    void Update() {

        // ** Update is called once per frame ** //

        deathCheck();
        surfaceCheck();
        animationUpdate();
        statusCheckAndUpdate();
        weaponCollisionCheck();
        lockingOnWithinDistanceUpdate();

    }

    private void FixedUpdate() {

        // ** Fixed Update is called 50 times per frame after Update() ** //

        if (playerNumber == 1) { debug(); }

        if (!controlsLocked) { controls(); }
        if (!movementLocked) { movement(); }

        dodgeRecovery();
        staminaManager();
        wallJumpManager();
        heavyAttackManager();

        if (isOnFire) { burningManager(); }
        if (isCaltropped) { caltroppedManager(); }

        if (itemTracker != null) {

            if (itemTracker.GetComponent<GrapplingHookBehaviour>() is GrapplingHookBehaviour) { grapplingHookManager(); }

        }

    }

    // ***          DETECTIONS           *** //

    private void OnTriggerEnter2D(Collider2D collision) {

        if (takeDamageFromPlayer) {

            if (!attackCollisionCheck.getWeaponHit() && (collision.CompareTag(Global.gameObjectTag_Weapon) || collision.CompareTag(Global.gameObjectTag_Enemy))) {

                if (!isDefending) {
                    health--;
                }
                else if (isDefending) {
                    currentStamina--;
                    playerDetailManager.switchStaminaAnimation(currentStamina);
                }

            }
            else if (!attackCollisionCheck.getWeaponHit() && collision.CompareTag(Global.gameObjectTag_HeavyAttack)) {

                health--;

            }

        }

        //Detect hit from Arrow
        if (collision.CompareTag(Global.gameObjectTag_Arrow))
        {
            health--;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        
        //Detects if player is attacked by another player or monster
        if (takeDamageFromPlayer) {

            if (!attackCollisionCheck.getWeaponHit() && (collision.collider.CompareTag(Global.gameObjectTag_Weapon) || collision.collider.CompareTag(Global.gameObjectTag_Enemy))) {

                if (!isDefending) {
                    health--;
                }
                else if (isDefending) {
                    currentStamina--;
                    playerDetailManager.switchStaminaAnimation(currentStamina);
                }

            }
            else if (!attackCollisionCheck.getWeaponHit() && collision.collider.CompareTag(Global.gameObjectTag_HeavyAttack)) {

                health--;

            }

        }

        //Detect hit from footspikes
        /*
        if (collision.collider.CompareTag(Global.gameObjectTag_FootSpikes)) {
            health--;
        }
        */

        //Detect hit from kunai
        if (collision.collider.CompareTag(Global.gameObjectTag_Kunai)) {

            playerRigidBody.velocity = Vector2.zero; //Prevent player from being pushed bu item

            health--;

        }

        //Detect hit from fire bomb
        if (collision.collider.CompareTag(Global.gameObjectTag_FireBomb)) {

            playerRigidBody.velocity = Vector2.zero; //Prevent player from being pushed bu item

            //Play burning animation

            if (!isOnFire) {
                isOnFire = true;
                burningCountdown = burningDuration;
            }
            else if (isOnFire) {
                health--;
            }

        }

        //Detect hit from shuriken
        if (collision.collider.CompareTag(Global.gameObjectTag_Shuriken)) {

            playerRigidBody.velocity = Vector2.zero; //Prevent player from being pushed bu item

            currentStamina--;
            playerDetailManager.switchStaminaAnimation(currentStamina);

            CurrentHeavyAttackCharge = 0;
            playerDetailManager.fillMeterBar(CurrentHeavyAttackCharge);

        }

    }

    // ***      PLAYER MANIPULATION       *** //


    private void jump() {

        if (Input.GetButton(Global.inputAxes_Jump + playerNumber) && currentJump != 0) {

            currentJump--;

            playerAnimator.SetBool(Global.playerAnimatorVariable_OnGround, false);

            // !!! To cancel the current momentum so that the player will jump accordingly to its jump force each time
            playerRigidBody.velocity = Vector2.zero;

            if (isOnWater || isCaltropped) {
                playerRigidBody.AddForce(new Vector2(0, jumpForce * (float)0.5));
            }
            else if (isOnWall) {
                wallJumpCooldownCounter++;
                playerRigidBody.AddForce(new Vector2(0, jumpForce * (float)1.5));
            }
            else {
                playerRigidBody.AddForce(new Vector2(0, jumpForce));
            }

        }

    }

    private void dodge() {

        if (Input.GetButtonDown(Global.inputAxes_Dodge + playerNumber) && dodgeCooldownCounter == 0) {

            currentStamina--;
            playerDetailManager.switchStaminaAnimation(currentStamina);

            dodgeCooldownCounter += (float)0.01;

            // !!! To cancel the current momentum so that the player will dodge accordingly to its push force each time
            playerRigidBody.velocity = Vector2.zero;

            playerAnimator.SetBool(Global.playerAnimatorVariable_Dodge, true);

            //Pushes based on the direction the player is facing
            if (isCaltropped) {
                if (lookingRight) { playerRigidBody.AddForce(new Vector2(-pushForce / 2, 0)); }
                else if (!lookingRight) { playerRigidBody.AddForce(new Vector2(pushForce / 2, 0)); }
            }
            else {
                if (lookingRight) { playerRigidBody.AddForce(new Vector2(-pushForce, 0)); }
                else if (!lookingRight) { playerRigidBody.AddForce(new Vector2(pushForce, 0)); }
            }

        }

    }

    private void parry() {

        if (Input.GetButton(Global.inputAxes_Parry + playerNumber) && isOnGround) {

            movementLocked = true;
            controlsLocked = true;
            playerAnimator.SetBool(Global.playerAnimatorVariable_Parry, true);

            attackCollisionCheck.setWeaponTagParrying();

        }

    }

    private void lockOn() {

        if (Input.GetButtonDown(Global.inputAxes_LockOn + playerNumber)) {

            if (lookingRight) {
                lockOnPlayerDetection = Physics2D.Raycast(transform.position, Vector2.right, lockOnRange, enemyObjects);
            }
            else {
                lockOnPlayerDetection = Physics2D.Raycast(transform.position, Vector2.left, lockOnRange, enemyObjects);
            }

            if (lockOnPlayerDetection.collider != null && !isLockingOnTarget) {

                if (lockOnPlayerDetection.collider.gameObject.CompareTag(Global.gameObjectTag_Player) || lockOnPlayerDetection.collider.gameObject.CompareTag(Global.gameObjectTag_Enemy)) {
                    isLockingOnTarget = true;
                }

            }

        }

    }

    private void defend() {

        if (Input.GetButton(Global.inputAxes_Defend + playerNumber) && isOnGround) {

            if (isDefending) {

                playerRigidBody.velocity = Vector2.zero; // !!! This is to prevent the player from moving and defending
                playerAnimator.SetFloat(Global.playerAnimatorVariable_Velocity, 0);

                controlsLocked = true;
                movementLocked = true;
                playerAnimator.SetBool(Global.playerAnimatorVariable_Defend, true);

            }
            else if (!isDefending) {

                controlsLocked = false;
                movementLocked = false;
                playerAnimator.SetBool(Global.playerAnimatorVariable_Defend, false);

            }


        }

    }

    private void attack() {

        if (Input.GetButton(Global.inputAxes_Attack + playerNumber) && currentStamina != 0 && isOnGround) {

            currentStamina--;

            movementLocked = true;
            controlsLocked = true;
            weapon.gameObject.SetActive(true);

            playerAnimator.SetBool(Global.playerAnimatorVariable_Attack, true);

            playerRigidBody.velocity = Vector2.zero; // !!! This is to prevent the player from moving and attacking
            playerDetailManager.switchStaminaAnimation(currentStamina);

        }

    }

    private void useItem() {

        if (Input.GetButton(Global.inputAxes_UseItem + playerNumber) && isOnGround) {

            if (!isUsingItem) { isUsingItem = true; }

        }

    }

    private void movement() {

        jump();
        dodge();
        horizontalMovement();

    }

    private void controls() {

        parry();
        lockOn();
        defend();
        attack();
        useItem();

    }

    private void horizontalMovement() {

        //Left-right movement

        if (Input.GetButton(Global.inputAxes_Horizontal + playerNumber) || (Mathf.Abs(Input.GetAxis(Global.inputAxes_Horizontal + playerNumber)) > 0)) {

            float horizontalMovement = 0f;

            wasMoving = true;

            if (isOnWater || isCaltropped) {
                horizontalMovement = (Input.GetAxis(Global.inputAxes_Horizontal + playerNumber) * movementSpeed) * (float)0.5;
            }
            else if (!isOnGround) {
                horizontalMovement = (Input.GetAxis(Global.inputAxes_Horizontal + playerNumber) * movementSpeed) * (float)1.3;
            }
            else {
                horizontalMovement = (Input.GetAxis(Global.inputAxes_Horizontal + playerNumber) * movementSpeed);
            }

            if (!isLockingOnTarget) {
                flipPlayer(horizontalMovement);
            }

            playerRigidBody.velocity = new Vector2(horizontalMovement, playerRigidBody.velocity.y);
            playerAnimator.SetFloat(Global.playerAnimatorVariable_Velocity, Mathf.Abs(Input.GetAxis(Global.inputAxes_Horizontal + playerNumber)));

        }
        else if (wasMoving) {

            // !!! Prevents player from sliding when not moving
            //Unity physics issue with physics material

            wasMoving = false;
            playerRigidBody.velocity = Vector2.zero;
            playerAnimator.SetFloat(Global.playerAnimatorVariable_Velocity, 0);

        }

    }

    private void flipPlayer(float horizontalMovement) {

        //Flip sprite based on looking direction
        if (horizontalMovement < 0) {

            //Flip to left

            lookingRight = false;

            playerRigidBody.transform.localScale = new Vector3(-Mathf.Abs(playerRigidBody.transform.localScale.x), playerRigidBody.transform.localScale.y, playerRigidBody.transform.localScale.z);

            //for flip back the PlayerDetail_Canvas
            transform.Find("PlayerDetails_Canvas").localScale = new Vector3(-Mathf.Abs(transform.Find("PlayerDetails_Canvas").localScale.x), transform.Find("PlayerDetails_Canvas").localScale.y, transform.Find("PlayerDetails_Canvas").localScale.z);

        }
        else if (horizontalMovement > 0) {

            //Flip to right

            lookingRight = true;

            playerRigidBody.transform.localScale = new Vector3(Mathf.Abs(playerRigidBody.transform.localScale.x), playerRigidBody.transform.localScale.y, playerRigidBody.transform.localScale.z);

            //for flip back the PlayerDetail_Canvas
            transform.Find("PlayerDetails_Canvas").localScale = new Vector3(Mathf.Abs(transform.Find("PlayerDetails_Canvas").localScale.x), transform.Find("PlayerDetails_Canvas").localScale.y, transform.Find("PlayerDetails_Canvas").localScale.z);

        }

    }

    // ***      ITEMS        *** //

    private void kunai() {

        KunaiBehaviour kunaiObjectManipulate = (Instantiate(kunaiObject)).GetComponent<KunaiBehaviour>();

        kunaiObjectManipulate.throwKunai(transform.position.x, transform.position.y, 1, lookingRight);

        itemAtHand = Global.items.itemNone;
        isUsingItem = false;

    }

    private void elixer() {

        switch ((int)itemUseCurrentDurationCounter) {

            case (int)smokeBombDuration:
                //Cancels the item usage when the timer is reached
                itemAtHand = Global.items.itemNone;
                isUsingItem = false;
                itemUseCurrentDurationCounter = 0;
                break;

            case 0:
                Debug.Log(1);
                elixerFX.SetActive(true);
                itemUseCurrentDurationCounter += (Time.deltaTime % 60);
                break;

            default:
                //Continues using item and counts the timer
                itemUseCurrentDurationCounter += (Time.deltaTime % 60);
                break;

        }

    }

    private void caltrop() {

        CaltropBehaviour caltropObjectManipulate = (Instantiate(caltropObject)).GetComponent<CaltropBehaviour>();

        caltropObjectManipulate.placeCaltrop(transform.position.x, transform.position.y, lookingRight);

        itemAtHand = Global.items.itemNone;
        isUsingItem = false;

    }

    private void fireBomb() {

        fireBombBehaviour fireBombObjectManipulate = (Instantiate(fireBombObject)).GetComponent<fireBombBehaviour>();

        fireBombObjectManipulate.throwFireBomb(transform.position.x, transform.position.y, 1, lookingRight);

        itemAtHand = Global.items.itemNone;
        isUsingItem = false;

    }

    private void shuriken() {

        ShurikenBehaviour shurikenObjectManipulate = (Instantiate(shurikenObject)).GetComponent<ShurikenBehaviour>();

        shurikenObjectManipulate.throwShuriken(transform.position.x, transform.position.y, 1, lookingRight);

        itemAtHand = Global.items.itemNone;
        isUsingItem = false;

    }

    private void phaseWalk() {

        switch ((int)itemUseCurrentDurationCounter) {

            case 0:
                portalsFX.SetActive(true);
                isAbleToPhaseWalk = true;
                break;

            case (int)phaseWalkDuration:

                //Cancels the item usage when the timer is reached

                isAbleToPhaseWalk = false;
                //turn off animation

                itemAtHand = Global.items.itemNone;
                isUsingItem = false;
                itemUseCurrentDurationCounter = 0;

                break;

            default:
                //Continues using item and counts the timer
                itemUseCurrentDurationCounter += Time.deltaTime;
                break;

        }

    }

    private void smokeBomb() {

        switch ((int)itemUseCurrentDurationCounter) {

            case 0:
                smokeFX.SetActive(true);
                this.gameObject.tag = Global.gameObjectTag_PlayerSmoked;
                break;

            case (int)smokeBombDuration:

                //Cancels the item usage when the timer is reached

                this.gameObject.tag = Global.gameObjectTag_Player;
                //turn off animation

                itemAtHand = Global.items.itemNone;
                isUsingItem = false;
                itemUseCurrentDurationCounter = 0;

                break;

            default:
                //Continues using item and counts the timer
                itemUseCurrentDurationCounter += Time.deltaTime;
                break;

        }

    }

    private void footSpikes() {

        switch ((int)itemUseCurrentDurationCounter) {

            case 0:
                footSpikesUse.gameObject.SetActive(true);
                break;

            case (int)footSpikesDuration:

                //Cancels the item usage when the timer is reached

                footSpikesUse.gameObject.SetActive(false);

                itemAtHand = Global.items.itemNone;
                isUsingItem = false;
                itemUseCurrentDurationCounter = 0;

                break;

            default:
                //Continues using item and counts the timer
                itemUseCurrentDurationCounter += Time.deltaTime;
                break;

        }

    }

    private void grapplingHook() {

        itemTracker = (Instantiate(grapplingHookObject));

        GrapplingHookBehaviour grapplingHookObjectManipulate = itemTracker.GetComponent<GrapplingHookBehaviour>();

        grapplingHookObjectManipulate.shootGrapplingHook(transform.position.x, transform.position.y);
        
        itemAtHand = Global.items.itemNone;
        isUsingItem = false;

        controlsLocked = true;
        movementLocked = true;

    }

    private void itemUsageManager() {

        //Checks which item the player has and uses it

        switch (itemAtHand) {

            case Global.items.kunai:
                kunai();
                break;

            case Global.items.caltrop:
                caltrop();
                break;

            case Global.items.fireBomb:
                fireBomb();
                break;

            case Global.items.elixer:
                elixer();
                break;

            case Global.items.shuriken:
                shuriken();
                break;

            case Global.items.phaseWalk:
                phaseWalk();
                break;

            case Global.items.smokeBomb:
                smokeBomb();
                break;

            case Global.items.footSpikes:
                footSpikes();
                break;

            case Global.items.grapplingHook:
                if (itemTracker == null) { grapplingHook(); }
                break;

        }

    }

    private void switchItemUI(int itemAtHandIndex) {

        playerDetailManager.switchCurrentItemNumber(itemAtHandIndex);

    }

    // ***      PLAYER STATS MANAGER           *** //

    private void burningManager() {

        switch ((int)burningCountdown) {

            case (0):
                //Stop burning animation
                isOnFire = false;
                health--;
                break;

            default:
                burningCountdown -= Time.deltaTime;
                break;

        }

    }

    private void staminaManager() {


        if (currentStamina == 0) {

            //Checks for total stamina depletion
            //Stamina will not recover at all untill 15 seconds

            switch ((int)staminaRecoveryCounter) {

                case (15): //(int)(staminaRecoveryDuration * 1.5)

                    //Stamina recovers directly to full in 15 seconds if fully depleted

                    staminaRecoveryCounter = 0f;
                    currentStamina = maxStamina;

                    staminaDepleted = false;

                    playerDetailManager.switchStaminaAnimation(currentStamina);

                    break;

                case 0:

                    staminaDepleted = true;

                    break;

                default:

                    //Counts each second and prevents stamina from adding

                    if (isUsingItem && itemAtHand == Global.items.elixer) {
                        //Increase heavy attack meter by double rate if using medicine item
                        staminaRecoveryCounter += Time.deltaTime * 2;
                    }
                    else {
                        //Increase heavy attack meter
                        staminaRecoveryCounter += Time.deltaTime;
                    }


                    break;

            }

        }
        else if (currentStamina != maxStamina && !staminaDepleted) {

            //Checks for non total stamina depletion

            switch ((int)staminaRecoveryCounter) {

                case (int)staminaRecoveryDuration:

                    //Adds a stamina once the duration is met
                    staminaRecoveryCounter = 0f;
                    currentStamina++;

                    playerDetailManager.switchStaminaAnimation(currentStamina);

                    break;

                default:

                    //Adds a seconds to the stamina recovery counter after every second
                    if (isUsingItem && itemAtHand == Global.items.elixer) {
                        //Increase heavy attack meter by double rate if using medicine item
                        staminaRecoveryCounter += Time.deltaTime * 2;
                    }
                    else {
                        //Increase heavy attack meter
                        staminaRecoveryCounter += Time.deltaTime;
                    }

                    break;

            }

        }

    }

    private void wallJumpManager() {

        switch ((int)wallJumpCooldownCounter) {

            case 0: //Checks if player is touching a wall
                isOnWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundObjects);
                break;

            case (int)wallJumpCooldownTime: //Reset wall jump cooldown timer
                wallJumpCooldownCounter = 0f;
                break;

            default: //Prevents player from wall jumping and counts the cooldown timer
                isOnWall = false;
                wallJumpCooldownCounter += Time.deltaTime;
                break;

        }

    }

    private void caltroppedManager() {

        switch ((int)caltroppedCooldown) {

            case ((int)caltroppedDuration):
                isCaltropped = false;
                break;

            default:
                caltroppedCooldown += Time.deltaTime;
                break;

        }

    }

    private void heavyAttackManager() {

        if (playerRigidBody.velocity == Vector2.zero && !heavyAttackReady) {

            //Checks if player is moving and heavy attack meter is not fully charged

            switch ((int)CurrentHeavyAttackCharge) {

                case ((int)heavyAttackRecoveryDuration):

                    //Sets heavy attack meter to fully charged

                    heavyAttackReady = true;

                    attackCollisionCheck.setWeaponTagHeavyAttack();

                    break;

                default:

                    if (isUsingItem && itemAtHand == Global.items.elixer) {
                        //Increase heavy attack meter by double rate if using medicine item
                        CurrentHeavyAttackCharge += Time.deltaTime * 2;
                    }
                    else {
                        //Increase heavy attack meter
                        CurrentHeavyAttackCharge += Time.deltaTime;
                    }

                    break;

            }


            playerDetailManager.fillMeterBar(CurrentHeavyAttackCharge);

        }
        else if (playerRigidBody.velocity != Vector2.zero && CurrentHeavyAttackCharge != 0) {

            //Checks if heavy attack meter is full

            CurrentHeavyAttackCharge -= Time.deltaTime * (float)1.25;

            if (heavyAttackReady) {

                heavyAttackReady = false;

                attackCollisionCheck.setWeaponTagDefault();

            }

            playerDetailManager.fillMeterBar((int)CurrentHeavyAttackCharge);

        }

    }

    private void grapplingHookManager() {

        GrapplingHookBehaviour grapplingHookObjectManipulate = itemTracker.GetComponent<GrapplingHookBehaviour>();

        if(grapplingHookObjectManipulate.getIsHooked()) {
            
            if(transform.position.y == grapplingHookObjectManipulate.getPositionY() || grapplingHookObjectManipulate == null) {
                
                controlsLocked = false;
                movementLocked = false;

                itemTracker = null;

                grapplingHookObjectManipulate.destroyObject();

            }
            else {

                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, grapplingHookObjectManipulate.getPullSpeed());

            }

        }

    }

    // ***          PLAYER OBJECT            *** //

    private void dodgeRecovery() {

        //This function is to get the player to stop sliding after dodging
        //and allow the player to dodge again

        if ((int)dodgeCooldownCounter == dodgeCooldownTime) { dodgeCooldownCounter = 0; }
        else if ((int)dodgeCooldownCounter == 1) { playerRigidBody.velocity = Vector2.zero; } // !!! This is to stop the player from sliding after dodging

        if (dodgeCooldownCounter > 0) { dodgeCooldownCounter += Time.deltaTime; }

    }

    private void destroyPlayerObject() { Destroy(gameObject); }

    // ***          TO CONSTANTLY CHECK         *** //

    private void deathCheck() {

        //Check if player goes out of bounce in map
        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, fallOffObjects)) {
            isDead = true;
            Destroy(gameObject);
        }

        //Checks if player is dead
        if (health == 0 && !isDead) {
            isDead = true;
            movementLocked = true;
            playerAnimator.SetTrigger(Global.playerAnimatorVariable_Death);
        }

    }

    private void surfaceCheck() {

        //Check if player is on ground
        isOnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundObjects);

        //Check if player is on water
        isOnWater = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, waterObjects);

    }

    private void animationUpdate() {

        //Makes sure Animation's onGround variable has the same onGround value
        if (isOnGround && !playerAnimator.GetBool(Global.playerAnimatorVariable_OnGround)) {
            playerAnimator.SetBool(Global.playerAnimatorVariable_OnGround, true);
        }
        else if (!isOnGround && playerAnimator.GetBool(Global.playerAnimatorVariable_OnGround)) {
            playerAnimator.SetBool(Global.playerAnimatorVariable_OnGround, false);
        }

    }

    private void statusCheckAndUpdate() {

        //If player is parried, player is able to parry back
        if (isParried) {
            parry();
        }

        //Checks if player is currently using and uses if needed
        if (isUsingItem) {
            Invoke("itemUsageManager", 0);
        }

        //If player is on fire and touches water surface, fire will be put out
        if (isOnWater && isOnFire) {
            isOnFire = false;
            burningCountdown = burningDuration;
        }

        //Reset player jump
        if ((isOnGround || isOnWall) && currentJump != maxPlayerJump) {
            currentJump = maxPlayerJump;
        }

    }

    private void weaponCollisionCheck() {

        //Check weapon's collision status
        if (attackCollisionCheck.getWeaponHit()) {

            //Push player back if player's sword collide with enemy's sword

            if (lookingRight) { playerRigidBody.AddForce(new Vector2((int)(-pushForce * 0.65), 0)); }
            else if (!lookingRight) { playerRigidBody.AddForce(new Vector2((int)(pushForce * 0.65), 0)); }

            dodgeCooldownCounter += (int)0.1;

            attackCollisionCheck.setWeaponHit(false);

        }
        else if (attackCollisionCheck.getEnemyParrying()) {

            //Sets the player to a parried state if being parried

            CurrentHeavyAttackCharge += heavyAttackRecoveryDuration / 2;

            movementLocked = true;
            controlsLocked = true;

            attackCollisionCheck.setEnemyParrying(false);
            isParried = true;

            playerAnimator.SetBool(Global.playerAnimatorVariable_Parried, true);

        }

    }

    private void lockingOnWithinDistanceUpdate() {

        //Cancel lock on after target is beyond radius
        if (isLockingOnTarget) {

            if (lookingRight) {
                lockOnPlayerDetection = Physics2D.Raycast(transform.position, Vector2.right, lockOnRange, enemyObjects);
            }
            else {
                lockOnPlayerDetection = Physics2D.Raycast(transform.position, Vector2.left, lockOnRange, enemyObjects);
            }

            if (lockOnPlayerDetection.collider == null) {
                isLockingOnTarget = false;
            }

        }

    }

    // ***          SETTER METHODS           *** //

    //Item Detector

    //Detect hit from caltrop
    public void caltropDetectedEvent()
    {

        isCaltropped = true;
        caltroppedCooldown = 1;

    }
    

    //Player State

    private void setIsParriedFalse() { isParried = false; }

    private void setLockMovementFalse() { movementLocked = false; }

    private void setControlsLockedFalse() { controlsLocked = false; }

    private void setPlayerIsDefendingFalse() { isDefending = false; }

    public void setPlayerHealth(int health) { this.health = health; }

    public void setPlayerNumber(int playerNumber) { this.playerNumber = playerNumber; }

    public void pushPlayer(float x, float y) { playerRigidBody.AddForce(new Vector2(-x, y)); }

    public void setPlayerPosition(float x, float y) { transform.position = new Vector2(x, y); }

    public void setItemAtHand(int itemIndex) { switchItemUI(itemIndex); itemAtHand = (Global.items)itemIndex; }

    //Weapon state

    private void setAttackCheckActiveFalse() { weapon.gameObject.SetActive(false); }

    private void setWeaponTagToDefault() { attackCollisionCheck.setWeaponTagDefault(); }

    private void setHeavyAttackFalse() { if (heavyAttackReady) { heavyAttackReady = false; } }

    //Animations

    //private void setTriggerDeathFalse() { playerAnimator.SetTrigger(Global.playerAnimatorVariable_Death); }

    private void setDodgeAnimationFalse() { playerAnimator.SetBool(Global.playerAnimatorVariable_Dodge, false); }

    private void setParryAnimationFalse() { playerAnimator.SetBool(Global.playerAnimatorVariable_Parry, false); }

    private void setAttackAnimationFalse() { playerAnimator.SetBool(Global.playerAnimatorVariable_Attack, false); }

    // ***          GETTER METHODS          *** //

    public bool getIsDead() { return isDead; }

    public bool getLookingRight() { return lookingRight; }

    public bool getIsUsingItem() { return isUsingItem; }

    public int getPlayerNumber() { return playerNumber; }

    public Global.items getItemAtHand() { return itemAtHand; }

    public bool getIsAbleToPhaseWalk() { return isAbleToPhaseWalk; }

    public float getPlayerPositionX() { return transform.position.x; }

    public float getPlayerPositionY() { return transform.position.y; }

    // ***          DEBUG            *** //

    private void debug() {

        /*
         
                            NOTE

                This method is used to debug.
                Place anything in here to quickly access item,
                regenerate stamina, fill heavy attack meter,
                score a point in multiplayer, proceed to next
                round in survival, etc.

        */

        itemGive_Debug();
        displayOnSceneWindow_Debug();
        manipulatePlayerStats_Debug();

    }

    private void itemGive_Debug() {

        //Instant item give
        if (Input.GetKey(KeyCode.Alpha1)) {
            Debug.Log("Item Caltrop now at hand");
            itemAtHand = Global.items.caltrop;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha2)) {
            Debug.Log("Item elixer now at hand");
            itemAtHand = Global.items.elixer;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha3)) {
            Debug.Log("Item fire bomb now at hand");
            itemAtHand = Global.items.fireBomb;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha4)) {
            Debug.Log("Item foot spikes now at hand");
            itemAtHand = Global.items.footSpikes;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha5)) {
            Debug.Log(isUsingItem);
            Debug.Log("Item kunai now at hand");
            itemAtHand = Global.items.kunai;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha6)) {
            Debug.Log("Item phase walk now at hand");
            itemAtHand = Global.items.phaseWalk;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha7)) {
            Debug.Log("Item shuriken now at hand");
            itemAtHand = Global.items.shuriken;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha8)) {
            Debug.Log("Item smoke bomb now at hand");
            itemAtHand = Global.items.smokeBomb;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha9)) {
            Debug.Log("Item grappling hook now at hand");
            itemAtHand = Global.items.grapplingHook;
            switchItemUI((int)itemAtHand);
        }
        else if (Input.GetKey(KeyCode.Alpha0)) {
            Debug.Log("Cleared all items at hand");
            itemAtHand = Global.items.itemNone;
            switchItemUI((int)itemAtHand);
        }

    }

    private void displayOnSceneWindow_Debug() {

        //Lock on range display
        if (lookingRight) {
            Debug.DrawRay(transform.position, Vector2.right * lockOnRange, Color.green);
        }
        else {
            Debug.DrawRay(transform.position, Vector2.left * lockOnRange, Color.green);
        }

    }

    private void manipulatePlayerStats_Debug() {

        if (Input.GetKey(KeyCode.K)) {
            Debug.Log("Killed yourself");
            isDead = true;
            Destroy(gameObject);
        }
        else if (Input.GetKey(KeyCode.Q)) {
            Debug.Log("Stamina increased");
            currentStamina++;
            playerDetailManager.switchStaminaAnimation(currentStamina);
        }
        else if (Input.GetKey(KeyCode.W)) {
            Debug.Log("Stamina maxed");
            currentStamina = maxStamina;
            playerDetailManager.switchStaminaAnimation(currentStamina);
        }
        else if (Input.GetKey(KeyCode.E)) {
            Debug.Log("Heavy attack charge increased");
            CurrentHeavyAttackCharge++;
            playerDetailManager.fillMeterBar((int)CurrentHeavyAttackCharge);
        }
        else if (Input.GetKey(KeyCode.R)) {
            Debug.Log("heavy attack charged");
            heavyAttackReady = true;
            CurrentHeavyAttackCharge = heavyAttackRecoveryDuration;
            playerDetailManager.fillMeterBar((int)CurrentHeavyAttackCharge);
        }

    }

}