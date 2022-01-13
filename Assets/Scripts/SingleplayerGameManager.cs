using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class SingleplayerGameManager : MonoBehaviour {

    [SerializeField] private Transform[] enemySpawnLocation;

    [SerializeField] private GameObject[] enemyObject;
    [SerializeField] private GameObject[] eventDialogBox;

    [SerializeField] private ObjectDetector[] checkpointLocation;
    [SerializeField] private ObjectDetector[] portalLocation;
    [SerializeField] private ObjectDetector[] eventTriggerLocation;
    [SerializeField] private ObjectDetector[] areaTransitionLocation;
    
    [SerializeField] private Animator[] portalAnimator;
    [SerializeField] private Animator[] savePointAnimator;

    [SerializeField] private Boss_Behaviour boss;
    [SerializeField] private GameObject[] bossItems;
    [SerializeField] private Transform BossTeleport_A;
    [SerializeField] private Transform BossTeleport_B;

    [SerializeField] private float trialsDuration = 300;

    [SerializeField] private InGameMenuLink inGameMenuLink;
    [SerializeField] private Player player;

    //Here
    //For take all of the checkpoints SavePointController.cs script Component
    private SavePointController[] savePointController = new SavePointController[Global.maxCheckPointInSinglePlayer];

    private List<GameObject> enemiesSpawned = new List<GameObject>();

    private int currentArea;
    private int currentScenario;
    private int lastCheckPointAt;

    private bool bossFight;
    private bool startTrials;
    private bool portalActive;

    private float trialsCountdownTimer;
    private float trialsEnemySpawnTimer;
    private float trialsEnemySpawnCounter;

    private void Awake() {

        currentArea = (int)Global.spArea.a;
        currentScenario = (int)Global.spScenario.event1;
        lastCheckPointAt = (int)Global.spCheckpointArea.a;

        bossFight = true;
        startTrials = false;
        portalActive = true;

        trialsCountdownTimer = trialsDuration;
        trialsEnemySpawnTimer = trialsDuration / 75;
        trialsEnemySpawnCounter = trialsEnemySpawnTimer;

    }

    private void Start() {

        FindObjectOfType<AudioManager>().playMusic("SinglePalyer_BGM");

        //Global.gameManager.setCurrentScene((int)Global.sceneMenuIndex.singlePlayer);

        //loadGame();
        initArea();

        if (portalActive) {
            activatePortals();
        }

        spawnAtCheckpoint();

        //Here
        //Take all of the checkpoints SavePointController.cs script Component from checkpointLocation[n]
        for (int i = 0; i < Global.maxCheckPointInSinglePlayer; i++)
        {
            savePointController[i] = checkpointLocation[i].GetComponent<Transform>().Find("SavePoint").GetComponent<SavePointController>();
        }
        

    }

    // Update is called once per frame
    private void Update() {

        //Check if player pause game
        if (Input.GetButtonDown(Global.inputAxes_Pause)) {
            inGameMenuLink.displayPauseMenu();
        }

        //Restart stage if player is dead
        if(player.getIsDead()) {
            reLoad();
        }
        
        if(startTrials) {
            trials();
        }

        scenarioStartCheck();
        areaTransitionCheck();
        portalCheck();

        //Here
        //Check which checkpoint are finish save condition
        checkSavedActionToSaveData();

    }

    private void scenarioStartCheck() {

        for (int i = 0; i < eventTriggerLocation.Length; i++) {

            if (eventTriggerLocation[i].getPlayerDetected() && currentScenario == i) {

                playScenario();
                break;

            }

        }

        if(boss == null) {
            playScenario();
        }

    }

    private void areaTransitionCheck() {

        for (int i = 0; i < areaTransitionLocation.Length; i++) {

            if (areaTransitionLocation[i].getPlayerDetected()) {

                areaTransition(i);
                break;

            }

        }

    }

    private void portalCheck() {
        
        if (portalActive) {

            for (int i = 0; i < portalLocation.Length; i++) {

                if (portalLocation[i].getPlayerDetected() && portalActive) {
                    
                    portalTransport(i);
                    break;

                }

            }

        }

    }

    private void saveCheckPoint() {
        
        for (int i = 0; i < checkpointLocation.Length; i++) {

            if (checkpointLocation[i].getPlayerDetected()) {
                lastCheckPointAt = i;
                break;
            }

        }

        saveGame();

    }

    private void saveGame() {
        
        FileStream file;
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        SaveData dataToSvae = new SaveData(lastCheckPointAt, currentArea, currentScenario, portalActive);

        if (File.Exists(Application.persistentDataPath + Global.saveFileName)) {
            file = File.Create(Application.persistentDataPath + Global.saveFileName);
        }
        else {
            file = File.Create(Application.persistentDataPath + Global.saveFileName);
        }

        binaryFormatter.Serialize(file, dataToSvae);

        file.Close();

    }

    private void loadGame() {

        FileStream file;
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + Global.saveFileName)) {
            file = File.OpenRead(Application.persistentDataPath + Global.saveFileName);
        }
        else {
            return;
        }

        SaveData dataToLoad = (SaveData)binaryFormatter.Deserialize(file);

        lastCheckPointAt = dataToLoad.getLastCheckPointAt();
        currentArea = dataToLoad.getCurrentArea();
        currentScenario = dataToLoad.getCurrentScenario();
        portalActive = dataToLoad.getPortalActive();

        file.Close();

    }

    private void reLoad() {

        SceneManager.LoadScene((int)Global.sceneMenuIndex.singlePlayer);

    }

    private void playScenario() {

        switch(currentScenario) {

            case (int)Global.spScenario.event1:
                eventDialogBox[0].GetComponent<DialogController>().startDialog((int)Global.spScenario.event1);
                //eventDialogBox[(int)Global.spScenario.event1].SetActive(true);
                break;

            case (int)Global.spScenario.event2:
                eventDialogBox[0].GetComponent<DialogController>().startDialog((int)Global.spScenario.event2);
                //eventDialogBox[(int)Global.spScenario.event2].SetActive(true);
                break;

            case (int)Global.spScenario.event3:
                eventDialogBox[0].GetComponent<DialogController>().startDialog((int)Global.spScenario.event3);
                //eventDialogBox[(int)Global.spScenario.event3].SetActive(true);
                break;

            case (int)Global.spScenario.event4:
                eventDialogBox[0].GetComponent<DialogController>().startDialog((int)Global.spScenario.event4);
                //eventDialogBox[(int)Global.spScenario.event4].SetActive(true);
                portalActive = true;
                activatePortals();
                break;

            case (int)Global.spScenario.event5:
                eventDialogBox[0].GetComponent<DialogController>().startDialog((int)Global.spScenario.event5);
                //eventDialogBox[(int)Global.spScenario.event5].SetActive(true);
                break;

            case (int)Global.spScenario.event6:
                eventDialogBox[0].GetComponent<DialogController>().startDialog((int)Global.spScenario.event6);
                //eventDialogBox[(int)Global.spScenario.event6].SetActive(true);
                //show end game menu
                break;

        }

        currentScenario++;

    }

    private void areaTransition(int transitionIndex) {

        //Area 1 transitions
        if(currentArea == (int)Global.spArea.area1 && transitionIndex == (int)Global.spAreaTransition.transition1) {
            //Transition from area 1 to area 2
            currentArea = (int)Global.spArea.area2;
        }
        else if(currentArea == (int)Global.spArea.area1 && transitionIndex == (int)Global.spAreaTransition.transition5) {
            //Transition from area 1 to area 6
            currentArea = (int)Global.spArea.area6;
        }

        //Area 2 transitions
        else if (currentArea == (int)Global.spArea.area2 && transitionIndex == (int)Global.spAreaTransition.transition1) {
            //Transition from area 2 to area 1
            currentArea = (int)Global.spArea.area1;
        }
        else if (currentArea == (int)Global.spArea.area2 && transitionIndex == (int)Global.spAreaTransition.transition2) {
            //Transition from area 2 to area 3
            currentArea = (int)Global.spArea.area3;
        }

        //Area 3 transitions
        else if (currentArea == (int)Global.spArea.area3 && transitionIndex == (int)Global.spAreaTransition.transition2) {
            //Transition from area 3 to area 2
            currentArea = (int)Global.spArea.area2;
        }
        else if (currentArea == (int)Global.spArea.area3 && transitionIndex == (int)Global.spAreaTransition.transition3) {
            //Transition from area 3 to area 4
            currentArea = (int)Global.spArea.area4;
        }

        //Area 4 transition
        else if (currentArea == (int)Global.spArea.area4 && transitionIndex == (int)Global.spAreaTransition.transition3) {
            //Transition from area 4 to area 3
            currentArea = (int)Global.spArea.area3;
        }
        else if (currentArea == (int)Global.spArea.area4 && transitionIndex == (int)Global.spAreaTransition.transition4) {
            //Transitionfrom area 4 to area 3
            currentArea = (int)Global.spArea.area3;
        }

        //Area 6A transition
        else if (currentArea == (int)Global.spArea.area6 && transitionIndex == (int)Global.spAreaTransition.transition5) {
            //Transition from area 6A to area 1
            currentArea = (int)Global.spArea.area1;
        }
        else if (currentArea == (int)Global.spArea.area6 && transitionIndex == (int)Global.spAreaTransition.transition6A) {
            //Transition from area 6A to 6B
            //Global.audioManager.playMusic(Global.music_Test); //Waterfall sound
        }

        //Area 6B transition
        else if (currentArea == (int)Global.spArea.area6 && transitionIndex == (int)Global.spAreaTransition.transition6B) {
            //Transition from area 6B to area 7
            currentArea = (int)Global.spArea.area7;
        }

        initArea();

    }

    private void initArea() {

        destroyEnemyObject();

        //Play bgm of area
        switch (currentArea) {

            case (int)Global.spArea.area1:
                //Global.audioManager.playMusic(Global.music_Test);
                break;

            case (int)Global.spArea.area2:
                //Global.audioManager.playMusic(Global.music_Test);
                break;

            case (int)Global.spArea.area3:
                //Global.audioManager.playMusic(Global.music_Test);
                break;

            case (int)Global.spArea.area4:
                //Global.audioManager.playMusic(Global.music_Test);
                break;

            case (int)Global.spArea.area5:
                startTrials = true;
                //Global.audioManager.playMusic(Global.music_Test);
                break;

            //Nothing to load for area 6

            case (int)Global.spArea.area7:
                //Global.audioManager.playMusic(Global.music_Test);
                break;

        }

        //Spawm enemies of area
        Invoke("enemyLocationSpawnManager", 0);

    }

    private void enemyLocationSpawnManager() {

        switch (currentArea) {

            case (int)Global.spArea.area1:
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area1_A].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area1_A].position.y, false, Global.typeOfEnemy.skeleton);
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area1_B].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area1_B].position.y, false, Global.typeOfEnemy.skeletonShield);
                break;

            case (int)Global.spArea.area2:
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_A].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_A].position.y, false, Global.typeOfEnemy.bat);
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_B].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_B].position.y, false, Global.typeOfEnemy.skeleton);
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_C].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_C].position.y, false, Global.typeOfEnemy.skeletonBrute);
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_D].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_D].position.y, true, Global.typeOfEnemy.skeletonArcher);
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_E].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area2_E].position.y, false, Global.typeOfEnemy.ghost);
                break;

            case (int)Global.spArea.area4:
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_A].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_A].position.y, true, Global.typeOfEnemy.skeletonBrute);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_B].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_B].position.y, false, Global.typeOfEnemy.skeletonBrute);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_B].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_B].position.y, true, Global.typeOfEnemy.skeletonBrute);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_C].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_C].position.y, false, Global.typeOfEnemy.skeletonBrute);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_C].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_C].position.y, true, Global.typeOfEnemy.skeletonBrute);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_D].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_D].position.y, false, Global.typeOfEnemy.skeletonBrute);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_D].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_D].position.y, true, Global.typeOfEnemy.skeletonBrute);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_E].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_E].position.y, false, Global.typeOfEnemy.skeletonBrute);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_E].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_E].position.y, true, Global.typeOfEnemy.skeletonBrute);
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_F].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_F].position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonShield);
                spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_G].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_G].position.y, false, Global.typeOfEnemy.skeletonBrute);
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_H].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_H].position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonShield);
                spawnEnemy(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_I].position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area4_I].position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.bat);
                break;

            //Script spawn for area 5 (trials)

            //Nothing to load for area 6

            case (int)Global.spArea.area7:
                boss.gameObject.SetActive(true);
                break;

        }

    }

    private void spawnEnemy(float x, float y, bool lookingRight, Global.typeOfEnemy enemyToSpawn) {

        enemySpawnManager((int)enemyToSpawn, x, y, lookingRight);

    }

    private void spawnEnemyRandom(float x, float y, bool lookingRight) {

        enemySpawnManager(UnityEngine.Random.Range(0, Enum.GetNames(typeof(Global.typeOfEnemy)).Length), x, y, lookingRight);

    }

    private void spawnEnemyRandom(float x, float y, bool lookingRight, Global.typeOfEnemy excludeEnemy) {
        
        int enemyRandom;

        while (true) {

            enemyRandom = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Global.typeOfEnemy)).Length);

            if (enemyRandom != (int)excludeEnemy) {
                break;
            }

        }

        enemySpawnManager(enemyRandom, x, y, lookingRight);

    }

    private void enemySpawnManager(int enemy, float x, float y, bool lookingRight) {

        switch (enemy) {

            case (int)Global.typeOfEnemy.bat:

                Bat_Behaviour batObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.bat])).GetComponent<Bat_Behaviour>();

                batObjectManipulate.spawnAt(x, y, lookingRight);

                enemiesSpawned.Add(batObjectManipulate.gameObject);

                break;

            case (int)Global.typeOfEnemy.ghost:

                Ghost_Behaviour ghostObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.ghost])).GetComponent<Ghost_Behaviour>();

                ghostObjectManipulate.spawnAt(x, y, lookingRight);

                enemiesSpawned.Add(ghostObjectManipulate.gameObject);

                break;

            case (int)Global.typeOfEnemy.skeleton:

                Skeleton_Behaviour skltnObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.skeleton])).GetComponent<Skeleton_Behaviour>();

                skltnObjectManipulate.spawnAt(x, y, lookingRight);

                enemiesSpawned.Add(skltnObjectManipulate.gameObject);

                break;

            case (int)Global.typeOfEnemy.skeletonArcher:

                SkeletonArcher_Behaviour skltnArrowObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.skeletonArcher])).GetComponent<SkeletonArcher_Behaviour>();

                skltnArrowObjectManipulate.spawnAt(x, y, lookingRight);

                enemiesSpawned.Add(skltnArrowObjectManipulate.gameObject);

                break;

            case (int)Global.typeOfEnemy.skeletonBrute:

                SkeletonBrute_Behaviour skltnBruteObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.skeletonBrute])).GetComponent<SkeletonBrute_Behaviour>();

                skltnBruteObjectManipulate.spawnAt(x, y, lookingRight);

                enemiesSpawned.Add(skltnBruteObjectManipulate.gameObject);

                break;

            case (int)Global.typeOfEnemy.skeletonShield:

                SkeletonShield_Behaviour skltnShieldObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.skeletonShield])).GetComponent<SkeletonShield_Behaviour>();

                skltnShieldObjectManipulate.spawnAt(x, y, lookingRight);

                enemiesSpawned.Add(skltnShieldObjectManipulate.gameObject);

                break;

            case (int)Global.typeOfEnemy.samurai:

                Boss_Behaviour bossObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.samurai])).GetComponent<Boss_Behaviour>();

                bossObjectManipulate.spawnAt(x, y, lookingRight);

                enemiesSpawned.Add(bossObjectManipulate.gameObject);

                break;

        }

    }

    private void spawnAtCheckpoint() {

        player.setPlayerPosition(checkpointLocation[lastCheckPointAt].transform.position.x, checkpointLocation[lastCheckPointAt].transform.position.y);

    }

    private void portalTransport(int portalIndex) {

        //To access trials area/room
        if(portalIndex == (int)Global.spPortals.portalG && currentScenario <= (int)Global.spScenario.event4) { // !!! switch to proper scenario !!!

            player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalH].transform.position.x-1, portalLocation[(int)Global.spPortals.portalH].transform.position.y);

            portalLocation[(int)Global.spPortals.portalH].gameObject.SetActive(false);

            currentArea = (int)Global.spArea.area5;
            initArea();

            return;

        }

        switch(portalIndex) {

            case (int)Global.spPortals.portalA:

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalB].transform.position.x-3, portalLocation[(int)Global.spPortals.portalB].transform.position.y);

                //portalLocation[(int)Global.spPortals.portalB].gameObject.SetActive(false);

                break;

            case (int)Global.spPortals.portalB:

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalA].transform.position.x+3, portalLocation[(int)Global.spPortals.portalA].transform.position.y);

                //portalLocation[(int)Global.spPortals.portalA].gameObject.SetActive(false);

                break;

            case (int)Global.spPortals.portalC:

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalD].transform.position.x-3, portalLocation[(int)Global.spPortals.portalD].transform.position.y);

                break;

            case (int)Global.spPortals.portalD:

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalC].transform.position.x+3, portalLocation[(int)Global.spPortals.portalC].transform.position.y);

                break;

            case (int)Global.spPortals.portalE:

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalF].transform.position.x-3, portalLocation[(int)Global.spPortals.portalF].transform.position.y);

                currentArea = (int)Global.spArea.area3;
                initArea();

                break;

            case (int)Global.spPortals.portalF:

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalE].transform.position.x-3, portalLocation[(int)Global.spPortals.portalE].transform.position.y);

                break;

            case (int)Global.spPortals.portalG:

                portalLocation[(int)Global.spPortals.portalJ].gameObject.SetActive(true);

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalJ].transform.position.x+3, portalLocation[(int)Global.spPortals.portalJ].transform.position.y);

                currentArea = (int)Global.spArea.area3;
                initArea();

                //portalLocation[(int)Global.spPortals.portalB].gameObject.SetActive(false);

                break;

            case (int)Global.spPortals.portalI:

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalJ].transform.position.x+3, portalLocation[(int)Global.spPortals.portalJ].transform.position.y);

                currentArea = (int)Global.spArea.area3;
                initArea();

                //portalLocation[(int)Global.spPortals.portalI].gameObject.SetActive(false);

                break;

            case (int)Global.spPortals.portalJ:

                player.setPlayerPosition(portalLocation[(int)Global.spPortals.portalG].transform.position.x + 3, portalLocation[(int)Global.spPortals.portalG].transform.position.y);

                currentArea = (int)Global.spArea.area1;
                initArea();

                //portalLocation[(int)Global.spPortals.portalG].gameObject.SetActive(false);

                break;

        }

    }

    private void trials() {

        switch((int)trialsCountdownTimer) {
             
            case 0:
                startTrials = false;
                portalLocation[(int)Global.spPortals.portalI].gameObject.SetActive(true);
                break;

            default:
                Invoke("trialsEnemySpawnManager", 0);
                trialsCountdownTimer -= Time.deltaTime;
                break;

        }
        
    }

    private void trialsEnemySpawnManager() {

        if(trialsEnemySpawnCounter <= 0) {

            switch(UnityEngine.Random.Range((int)Global.spEnemySpawnLocation.area5_A, (int)Global.spEnemySpawnLocation.area5_D)) {

                case (int)Global.spEnemySpawnLocation.area5_A:
                    spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_A].transform.position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_A].transform.position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonBrute);
                    break;

                case (int)Global.spEnemySpawnLocation.area5_B:
                    spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_B].transform.position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_B].transform.position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonBrute);
                    break;

                case (int)Global.spEnemySpawnLocation.area5_C:
                    spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_C].transform.position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_C].transform.position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonBrute);
                    break;

                //Spawn D onwards has been disabled

                case (int)Global.spEnemySpawnLocation.area5_D:
                    spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_D].transform.position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_D].transform.position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonBrute);
                    break;

                case (int)Global.spEnemySpawnLocation.area5_E:
                    spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_E].transform.position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_E].transform.position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonBrute);
                    break;

                case (int)Global.spEnemySpawnLocation.area5_F:
                    spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_F].transform.position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_F].transform.position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonBrute);
                    break;

                case (int)Global.spEnemySpawnLocation.area5_G:
                    spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_G].transform.position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_G].transform.position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonBrute);
                    break;

                case (int)Global.spEnemySpawnLocation.area5_H:
                    spawnEnemyRandom(enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_H].transform.position.x, enemySpawnLocation[(int)Global.spEnemySpawnLocation.area5_H].transform.position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.skeletonBrute);
                    break;

            }
            
            trialsEnemySpawnCounter = trialsEnemySpawnTimer;

        }
        else {
            trialsEnemySpawnCounter -= Time.deltaTime;
        }

    }

    private void destroyEnemyObject() {
        
        if (enemiesSpawned.Count != 0) {
            
            for (int i = 0; i < enemiesSpawned.Count; i++) {

                //Destroy the enemy object
                Destroy(enemiesSpawned[i]);

            }
            
            //Reset the number of enemies spawned in previous area
            enemiesSpawned.Clear();

        }
        
    }

    private void activatePortals() {

        portalActive = true;

        for(int i = 0; i < portalLocation.Length; i++) {

            if(i != (int)Global.spPortals.portalI) {
                portalLocation[i].gameObject.SetActive(true);
            }

        }

    }

    public void nextScenario() {

        currentScenario++;

    }

    //Here
    //For take which checkpoint position are player trigger saved condition
    private void checkSavedActionToSaveData() {

        for(int i=0; i< Global.maxCheckPointInSinglePlayer; i++) {

            if(savePointController[i].getSavedState() == true) {

                saveCheckPoint();
                savePointController[i].setSavedState(false);

            }

        }

    }
    
    public float getPlayerPositionX() {

        return player.getPlayerPositionX();

    }

    public float getPlayerPositionY() {

        return player.getPlayerPositionY();

    }

    public float getBossPhase1TeleportA_X() {

        return BossTeleport_A.position.x;

    }

    public float getBossPhase1TeleportA_Y() {

        return BossTeleport_A.position.y;

    }

    public float getBossPhase1TeleportB_X() {

        return BossTeleport_B.position.x;

    }

    public float getBossPhase1TeleportB_Y() {

        return BossTeleport_B.position.y;

    }
    
    public float getBossAreaMiddlePosX() {

        return enemySpawnLocation[(int)Global.spEnemySpawnLocation.area7_A].position.x;

    }

    public float getBossAreaMiddlePosY() {

        return enemySpawnLocation[(int)Global.spEnemySpawnLocation.area7_A].position.y;

    }

    public bool playBossFight() {

        return bossFight;

    }

    public GameObject getBossItem(Global.bossItem item) {

        return bossItems[(int)item];

    }

}
