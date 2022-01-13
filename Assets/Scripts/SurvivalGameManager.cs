using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System;

public class SurvivalGameManager : MonoBehaviour {

    [SerializeField] private InGameMenuLink inGameMenuLink;
    [SerializeField] private Transform[] spawnLocation;
    [SerializeField] private GameObject[] enemyObject;
    [SerializeField] private Player[] player;
    [SerializeField] private Text roundText;

    [SerializeField] private float roundDuration = 300; //3 Seconds

    private GameObject enemyToSpawn;

    private bool gameEnd;

    private float enemySpawnTimer;
    private float roundTimerCountdown;
    private float enemySpawnCountdown;

    private int round;

    private int numOfPlayers;
    private int playersAlive;

    //Value for show when game over
    private int[] playerScore = new int[Global.maxPlayersInSurvival];
    private int[] playerKill = new int[Global.maxPlayersInSurvival];

    //Texts get from menu
    public Text[] scoreText;
    public Text[] killText;

    public Slider loadingSlider;

    private void Awake() {

        gameEnd = false;

        round = 1;

        roundTimerCountdown = roundDuration;

        enemySpawnTimer = roundDuration / (round * 100);
        enemySpawnCountdown = enemySpawnTimer;

        numOfPlayers = 0;
        playersAlive = 0;

        for(int i = 0; i < Global.maxPlayersInSurvival; i++) {

            playerScore[i] = 0;
            playerKill[i] = 0;

            //scoreText[i].text = playerScore[i];

        }

        roundText.text = "" + round;

        Invoke("enemyLocationSpawnManager", 5);

    }

    // Use this for initialization
    private void Start () {

        FindObjectOfType<AudioManager>().playMusic("Survival_BGM");

        numOfPlayers = Global.gameManager.getTotalPlayersAssigned();

        for (int i = 0; i < Global.maxPlayersInSurvival; i++) {

            if (i < numOfPlayers) {
                player[i].setPlayerNumber(Global.gameManager.getAssignedPlayer(i));
            }
            else if (i >= numOfPlayers) {
                Destroy(player[i].gameObject);
                playerScore[i] = -1;
            }
            
        }

        Global.audioManager.playMusic(Global.music_Test);

    }
	
	// Update is called once per frame
	private void Update () {
        
        if (Input.GetButtonDown(Global.inputAxes_Pause)) {
            inGameMenuLink.displayPauseMenu();
        }

        playersAlive = numOfPlayers;

        for (int i = 0; i < numOfPlayers; i++) {

            if (player[i] == null) {
                playersAlive--;
            }

        }

        if (playersAlive == 0 && !gameEnd) {

            gameEnd = true;

            for(int i = 0; i < numOfPlayers;  i++) {

                inGameMenuLink.setSurvivalPlayersScoreText(i, playerScore[i]);
                inGameMenuLink.showEndMatchMenu();

            }

        }

    }

    private void FixedUpdate() {

        enemySpawnCountdown -= Time.deltaTime;
        roundTimerCountdown -= Time.deltaTime;

        if(roundTimerCountdown <= 0) {

            round++;
            roundText.text = "" + round;
            roundTimerCountdown = roundDuration;

            enemySpawnTimer = roundDuration / (round * 100);
            enemySpawnCountdown = enemySpawnTimer;
            

        }
        
        if(enemySpawnCountdown <= 0) {

            Invoke("enemyLocationSpawnManager", 0);

            enemySpawnCountdown = enemySpawnTimer;

        }

        for (int i = 0; i < playerScore.Length; i++)
        {
            Debug.Log(" ");
            Debug.Log("Scope: ");
            Debug.Log(playerScore[i]);
            Debug.Log(" ");
            Debug.Log("Kill: ");
            Debug.Log(playerKill[i]);
        }

    }

    private void enemyLocationSpawnManager() {

        //System.Random rng = new System.Random();

        switch (UnityEngine.Random.Range(0, Enum.GetNames(typeof(Global.svSpawnLocation)).Length)) { //UnityEngine.Random.Range(0, Enum.GetNames(typeof(Global.svSpawnLocation)).Length)

            case (int)Global.svSpawnLocation.spawnA:
                enemyToSpawnManager(spawnLocation[(int)Global.svSpawnLocation.spawnA].position.x, spawnLocation[(int)Global.svSpawnLocation.spawnA].position.y, true);
                break;

            case (int)Global.svSpawnLocation.spawnB:
                enemyToSpawnManager(spawnLocation[(int)Global.svSpawnLocation.spawnB].position.x, spawnLocation[(int)Global.svSpawnLocation.spawnB].position.y, false);
                break;

            case (int)Global.svSpawnLocation.spawnC:
                enemyToSpawnManager(spawnLocation[(int)Global.svSpawnLocation.spawnC].position.x, spawnLocation[(int)Global.svSpawnLocation.spawnC].position.y, Convert.ToBoolean(UnityEngine.Random.Range(0, 1)), Global.typeOfEnemy.bat, Global.typeOfEnemy.ghost);
                break;

            case (int)Global.svSpawnLocation.spawnD:
                enemyToSpawnManager(spawnLocation[(int)Global.svSpawnLocation.spawnD].position.x, spawnLocation[(int)Global.svSpawnLocation.spawnD].position.y, false, Global.typeOfEnemy.skeletonBrute);
                break;

            case (int)Global.svSpawnLocation.spawnE:
                enemyToSpawnManager(spawnLocation[(int)Global.svSpawnLocation.spawnE].position.x, spawnLocation[(int)Global.svSpawnLocation.spawnE].position.y, true);
                break;

        }

    }

    private void enemyToSpawnManager(float x, float y, bool lookingRight) {

        spawnEnemy(UnityEngine.Random.Range(0, Enum.GetNames(typeof(Global.typeOfEnemy)).Length), x, y, lookingRight);

    }

    private void enemyToSpawnManager(float x, float y, bool lookingRight, Global.typeOfEnemy excludeEnemy) {

        int enemyRandom;

        while (true) {

            enemyRandom = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Global.typeOfEnemy)).Length);

            if (enemyRandom != (int)excludeEnemy) {
                break;
            }

        }

        spawnEnemy(enemyRandom, x, y, lookingRight);

    }

    private void enemyToSpawnManager(float x, float y, bool lookingRight, Global.typeOfEnemy excludeEnemy1, Global.typeOfEnemy excludeEnemy2) {

        int enemyRandom;

        while (true) {

            enemyRandom = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Global.typeOfEnemy)).Length);

            if (enemyRandom != (int)excludeEnemy1 && enemyRandom != (int)excludeEnemy2) {
                break;
            }

        }

        spawnEnemy(enemyRandom, x, y, lookingRight);



    }

    private void spawnEnemy(int enemy, float x, float y, bool lookingRight) {

        switch (enemy) {

            case (int)Global.typeOfEnemy.bat:

                Bat_Behaviour batObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.bat])).GetComponent<Bat_Behaviour>();

                batObjectManipulate.spawnAt(x, y, lookingRight);

                break;

            case (int)Global.typeOfEnemy.ghost:

                Ghost_Behaviour ghostObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.ghost])).GetComponent<Ghost_Behaviour>();

                ghostObjectManipulate.spawnAt(x, y, lookingRight);

                break;

            case (int)Global.typeOfEnemy.skeleton:

                Skeleton_Behaviour skltnObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.skeleton])).GetComponent<Skeleton_Behaviour>();

                skltnObjectManipulate.spawnAt(x, y, lookingRight);

                break;

            case (int)Global.typeOfEnemy.skeletonArcher:

                SkeletonArcher_Behaviour skltnArrowObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.skeletonArcher])).GetComponent<SkeletonArcher_Behaviour>();

                skltnArrowObjectManipulate.spawnAt(x, y, lookingRight);

                break;

            case (int)Global.typeOfEnemy.skeletonBrute:

                SkeletonBrute_Behaviour skltnBruteObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.skeletonBrute])).GetComponent<SkeletonBrute_Behaviour>();

                skltnBruteObjectManipulate.spawnAt(x, y, lookingRight);

                break;

            case (int)Global.typeOfEnemy.skeletonShield:

                SkeletonShield_Behaviour skltnShieldObjectManipulate = (Instantiate(enemyObject[(int)Global.typeOfEnemy.skeletonShield])).GetComponent<SkeletonShield_Behaviour>();

                skltnShieldObjectManipulate.spawnAt(x, y, lookingRight);

                break;

        }

    }

    public bool checkIfSpecificPlayerIsDead(int playerNum) {

        if (playerNum < numOfPlayers) {

            if (player[playerNum].getIsDead()) {
                return true;
            }

        }

        return false;

    }

    public void setNumberOfPlayers(int numOfPlayers) {
        this.numOfPlayers = numOfPlayers;
    }

    public int getNumberOfPlayer() {
        return numOfPlayers;
    }

    public int getPlayerScore(int playerNum) {

        if(playerNum < numOfPlayers) {
            return playerScore[playerNum];
        }

        return -1;

    }

}
