using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePointController : MonoBehaviour {

    private enum playerNumberIndex { player01 = 1, player02 = 2, player03 = 3, player04 = 4 };

    [SerializeField] private float timerOfCharging = 0f;
    [SerializeField] private float getItemTimeChargeRequire = 3f;
    [SerializeField] private float finishSavedDelayTime = 2f;
    [SerializeField] private int singlePlayerCheckpointAreaIndex;

    private Animator savePointActiveAnimator;
    private Animator savePointAbleToPressAnimator;

    private bool havePlayersInSavePointTriggerArea;

    private bool[] whichPlayersInArea;

    private int currentSaveDataPlayerNumber;

    private bool savedState;

    // Use this for initialization
    private void Start() {

        savePointActiveAnimator = transform.GetComponent<Animator>();
        savePointAbleToPressAnimator = transform.Find("AblePressButtonEffect").GetComponent<Animator>();

        whichPlayersInArea = new bool[4] { false, false, false, false };

    }

    private void FixedUpdate() {

        if(savedState == false) {

            checkAbleToShowButtonAnimator();
            reducePickupCharge();
            hideButtonIfPlayerIsDead();

        }

    }

    private void OnTriggerStay2D(Collider2D collision) {

        if (collision.CompareTag(Global.gameObjectTag_Player) && savedState == false) {

            int playerNumber = collision.GetComponent<Player>().getPlayerNumber();

            getWhichPlayersInArea(playerNumber, true);

            switch (inputButtonToSavingData(playerNumber)) {

                case false:
                    checkAbleToShowButtonAnimator();
                    break;

                case true:
                    savedTheData();
                    Debug.Log("Saved!");

                    break;

            }

        }

    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.CompareTag(Global.gameObjectTag_Player)) {

            getWhichPlayersInArea(collision.GetComponent<Player>().getPlayerNumber(), false);

        }

    }

    //To get which player in the item area
    private void getWhichPlayersInArea(int playerIndex, bool playerInArea) {

        switch (playerIndex) {

            case (int)playerNumberIndex.player01:
                whichPlayersInArea[playerIndex - 1] = playerInArea;
                break;

            case (int)playerNumberIndex.player02:
                whichPlayersInArea[playerIndex - 1] = playerInArea;
                break;

            case (int)playerNumberIndex.player03:
                whichPlayersInArea[playerIndex - 1] = playerInArea;
                break;

            case (int)playerNumberIndex.player04:
                whichPlayersInArea[playerIndex - 1] = playerInArea;
                break;

            default:
                Debug.Log("The player0" + playerIndex + " is a default player!");
                break;

        }

    }

    private void hideButtonIfPlayerIsDead() {

        //Turn off item pickup button when player is dead

        //This section is done quickly, please improve it if you have any idea

        if (FindObjectOfType<GameManager>().getCurrrentGameMode() == Global.gameModes.multiPlayer) {

            for (int i = 0; i < Global.maxPlayersInMultiplayer; i++) {

                if (FindObjectOfType<MultiplayerGameManager>().checkIfSpecificPlayerIsDead(i)) {

                    getWhichPlayersInArea(i + 1, false);

                }

            }

        }
        else if (FindObjectOfType<GameManager>().getCurrrentGameMode() == Global.gameModes.survival) {

            for (int i = 0; i < Global.maxPlayersInMultiplayer; i++) {

                if (FindObjectOfType<SurvivalGameManager>().checkIfSpecificPlayerIsDead(i)) {

                    getWhichPlayersInArea(i + 1, false);

                }

            }

        }

    }

    //To check have player in the Save Point area or not have, 
    //if have player in area the show button animation, otherwise do not show
    private void checkAbleToShowButtonAnimator() {

        int howManyPlayersInArea = 0;

        for (int i = 0; i < whichPlayersInArea.Length; i++) {

            if (whichPlayersInArea[i] == true) {

                howManyPlayersInArea += 1;

            }

        }

        if (howManyPlayersInArea > 0) {

            havePlayersInSavePointTriggerArea = true;

        }
        else {

            havePlayersInSavePointTriggerArea = false;

        }

        savePointAbleToPressAnimator.SetBool("AbleToSaveData", havePlayersInSavePointTriggerArea);

    }

    //To check the player input and only allow one player pressing,
    private bool inputButtonToSavingData(int playerNumber) {

        Slider slider = transform.Find("SavePoint_Canvas/Slider").GetComponent<Slider>();

        if (Input.GetButton(Global.inputAxes_SelectDecline + playerNumber) && ((currentSaveDataPlayerNumber == 0 || currentSaveDataPlayerNumber == playerNumber) && havePlayersInSavePointTriggerArea) ) {

            if (!transform.Find("SavePoint_Canvas").GetComponent<Canvas>().gameObject.activeSelf) {

                transform.Find("SavePoint_Canvas").GetComponent<Canvas>().gameObject.SetActive(true);

            }

            currentSaveDataPlayerNumber = playerNumber;

            if(savedState == false)
            {
                timerOfCharging += Time.deltaTime;
            }
                

            slider.value = timerOfCharging;

            if (timerOfCharging > getItemTimeChargeRequire) {

                return true;

            }

        }
        else if (Input.GetButtonUp(Global.inputAxes_SwitchItem + playerNumber) && currentSaveDataPlayerNumber == playerNumber) {

            currentSaveDataPlayerNumber = 0;

        }

        return false;

    }

    private void reducePickupCharge() {

        if (timerOfCharging > 0 && !havePlayersInSavePointTriggerArea) {

            //Slowly reduce pickup charge when no player is picking up

            Slider slider = transform.Find("SavePoint_Canvas/Slider").GetComponent<Slider>();

            timerOfCharging -= (Time.deltaTime * 2);

            slider.value = timerOfCharging;

            if (transform.Find("SavePoint_Canvas/Text").GetComponent<Text>().gameObject.activeSelf) {

                transform.Find("SavePoint_Canvas/Text").GetComponent<Text>().gameObject.SetActive(false);

            }

            if (timerOfCharging <= 0) {

                transform.Find("SavePoint_Canvas").GetComponent<Canvas>().gameObject.SetActive(false);

            }

        }

    }

    private void savedTheData() {

        savePointActiveAnimator.SetBool("Active", true);

        savedState = true;

        Slider slider = transform.Find("SavePoint_Canvas/Slider").GetComponent<Slider>();

        timerOfCharging = 0f;

        slider.value = timerOfCharging;

        slider.gameObject.SetActive(false);

        if (!transform.Find("SavePoint_Canvas/Text").GetComponent<Text>().gameObject.activeSelf) {

            transform.Find("SavePoint_Canvas/Text").GetComponent<Text>().gameObject.SetActive(true);

        }

        savePointAbleToPressAnimator.SetBool("AbleToSaveData", false);

        StartCoroutine(delayTimer());

    }

    public void switchAnimationToUnActive() {

        savePointActiveAnimator.SetBool("Active", false);

    }

    IEnumerator delayTimer() {

        for (float i = 0; i < finishSavedDelayTime; i += Time.deltaTime) {

            yield return 0;

        }

        switchAnimationToUnActive();

        if (!transform.Find("SavePoint_Canvas/Slider").GetComponent<Slider>().gameObject.activeSelf) {

            transform.Find("SavePoint_Canvas/Slider").GetComponent<Slider>().gameObject.SetActive(true);

        }

        if (transform.Find("SavePoint_Canvas/Text").GetComponent<Text>().gameObject.activeSelf) {

            transform.Find("SavePoint_Canvas/Text").GetComponent<Text>().gameObject.SetActive(false);

        }

        if (transform.Find("SavePoint_Canvas").GetComponent<Canvas>().gameObject.activeSelf) {

            transform.Find("SavePoint_Canvas").GetComponent<Canvas>().gameObject.SetActive(false);

        }

        //savedState = false;
    }


    public void setSavedState( bool savedState )
    {
        this.savedState = savedState;
    }

    public bool getSavedState() { return savedState; }
    
}
