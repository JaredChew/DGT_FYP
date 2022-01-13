using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour {

    //private int fps = 60;

    private enum playerNumberIndex { player01 = 1, player02 = 2, player03 = 3, player04 = 4 };

    [SerializeField] private ParticleSystem getingEventParticleSystem;

    [SerializeField] private float timerOfCharging = 0f;
    [SerializeField] private float getItemTimeChargeRequire = 3f;

    private Animator itemAbleToGetButtonAnimator;

    private bool havePlayersInItemTriggerArea;

    private bool[] whichPlayersInArea;

    private int currentTakeItemPlayerNumber;

    // Use this for initialization
    private void Start() {

        itemAbleToGetButtonAnimator = transform.Find("AblePressButtonEffect").GetComponent<Animator>();

        whichPlayersInArea = new bool[4] { false, false, false, false };

    }

    private void FixedUpdate() {

        checkAbleToShowButtonAnimator();
        reducePickupCharge();
        hideButtonIfPlayerIsDead();

    }

    private void OnTriggerStay2D(Collider2D collision) {

        if ( collision.CompareTag(Global.gameObjectTag_Player)) {

            int playerNumber = collision.GetComponent<Player>().getPlayerNumber();

            getWhichPlayersInArea( playerNumber , true );

            switch ( inputButtonToTakeTtemAndGetItemType(playerNumber) ) { 

                case false:
                    checkAbleToShowButtonAnimator();
                    break;

                case true:
                    int itemGenerated = new System.Random().Next(1, Global.maxItemAmount);

                    Debug.Log(itemGenerated);
                    destoryItem();
                    collision.GetComponent<Player>().setItemAtHand(itemGenerated);
                    break;

            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision) {

        if ( collision.CompareTag(Global.gameObjectTag_Player) ) {

            getWhichPlayersInArea( collision.GetComponent<Player>().getPlayerNumber(), false );

        }

    }

    //To get which player in the item area
    private void getWhichPlayersInArea ( int playerIndex, bool playerInArea ) {

        switch (playerIndex)
        {
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
        else if(FindObjectOfType<GameManager>().getCurrrentGameMode() == Global.gameModes.survival) {

            for (int i = 0; i < Global.maxPlayersInMultiplayer; i++) {

                if (FindObjectOfType<SurvivalGameManager>().checkIfSpecificPlayerIsDead(i)) {
                    getWhichPlayersInArea(i + 1, false);
                }

            }

        }

    }

    //To check have player in the item area or not have, 
    //if have player in area the show button animation, otherwise do not show
    private void checkAbleToShowButtonAnimator()
    {

        int howManyPlayersInArea = 0;

        for (int i = 0; i < whichPlayersInArea.Length; i++)
        {

            if (whichPlayersInArea[i] == true)
            {

                howManyPlayersInArea += 1;

            }

        }

        if (howManyPlayersInArea > 0)
        {

            havePlayersInItemTriggerArea = true;

        }
        else
        {

            havePlayersInItemTriggerArea = false;

        }

        itemAbleToGetButtonAnimator.SetBool("AbleToGetItem", havePlayersInItemTriggerArea);

    }

    //To check the player input and only allow one player taking,
    //also return itemType to current take item player and destroy gameObject when timer charge finish.
    private bool inputButtonToTakeTtemAndGetItemType( int playerNumber ) {

        Slider slider = transform.Find("ItemCharging_Canvas/Slider").GetComponent<Slider>();

        if ( Input.GetButton(Global.inputAxes_SelectDecline + playerNumber) && ( (currentTakeItemPlayerNumber == 0 || currentTakeItemPlayerNumber == playerNumber) && havePlayersInItemTriggerArea) ) {

            if (!transform.Find("ItemCharging_Canvas").GetComponent<Canvas>().gameObject.activeSelf)
            {
                transform.Find("ItemCharging_Canvas").GetComponent<Canvas>().gameObject.SetActive(true);
            }
            
            currentTakeItemPlayerNumber = playerNumber;
            timerOfCharging += Time.deltaTime;

            slider.value = timerOfCharging;

            if (timerOfCharging > getItemTimeChargeRequire) {

                return true;

            }

        }
        else if (Input.GetButtonUp(Global.inputAxes_SwitchItem + playerNumber) && currentTakeItemPlayerNumber == playerNumber) {

            currentTakeItemPlayerNumber = 0;

        }

        return false;

    }

    private void reducePickupCharge() {

        if (timerOfCharging > 0 && !havePlayersInItemTriggerArea) {

            //Slowly reduce pickup charge when no player is picking up

            Slider slider = transform.Find("ItemCharging_Canvas/Slider").GetComponent<Slider>();

            timerOfCharging -= Time.deltaTime * 2;

            slider.value = timerOfCharging;

            if(timerOfCharging <= 0) {
                transform.Find("ItemCharging_Canvas").GetComponent<Canvas>().gameObject.SetActive(false);
            }

        }

    }

    public void destoryItem()
    {

        //Destroy(gameObject);
        transform.gameObject.SetActive(false);
        Instantiate( getingEventParticleSystem , transform.position, getingEventParticleSystem.transform.rotation );

    }

}
