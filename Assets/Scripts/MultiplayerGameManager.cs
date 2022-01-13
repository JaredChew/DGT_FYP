using UnityEngine;

public class MultiplayerGameManager : MonoBehaviour {

    [SerializeField] private InGameMenuLink inGameMenuLink;
    [SerializeField] private Player[] player;

    private bool gameEnd;

    private int playersDead;
    private int numOfPlayers;
    private int playerNumWinner;

    private void Awake () {

        gameEnd = false;

        playersDead = 0;
        numOfPlayers = 0;
        playerNumWinner = Global.maxPlayersInMultiplayer;

    }

    private void Start() {

        FindObjectOfType<AudioManager>().playMusic("MultiPalyer_BGM");

        numOfPlayers = Global.gameManager.getTotalPlayersAssigned();

        for (int i = 0; i < Global.maxPlayersInMultiplayer; i++) {

            if(i < numOfPlayers) {
                player[i].setPlayerNumber(Global.gameManager.getAssignedPlayer(i));
            }
            else if(i >= numOfPlayers) {
                Destroy(player[i].gameObject);
            }

        }

        switch(Global.gameManager.getCurrentScene()) {

            case Global.sceneMenuIndex.multiPlayer_L1:
                Global.audioManager.playMusic(Global.music_Test);
                break;

            case Global.sceneMenuIndex.multiPlayer_L2:
                //Play music
                break;

            case Global.sceneMenuIndex.multiPlayer_L3:
                //Play music
                break;

            case Global.sceneMenuIndex.multiPlayer_L4:
                //Play music
                break;

            case Global.sceneMenuIndex.multiPlayer_L5:
                //Play music
                break;

        }

    }

    // Update is called once per frame
    private void Update () {

        if (Input.GetButtonDown(Global.inputAxes_Pause)) {
            inGameMenuLink.displayPauseMenu();
        }

        playersDead = 0;

        for (int i = 0; i < numOfPlayers; i++) {

            if(player[i] == null) {
                playersDead++;
            }

        }

        if(playersDead == numOfPlayers - 1 && !gameEnd) {

            gameEnd = true;

            for (int i = 0; i < numOfPlayers; i++) {

                if(player[i] != null) {
                    playerNumWinner = i + 1;
                    break;
                }

            }

            inGameMenuLink.setMultiplayerWinnerText(playerNumWinner);
            inGameMenuLink.showEndMatchMenu();

        }

	}

    private void reset() {

        playersDead = 0;

        for(int i = 0; i < Global.maxPlayersInMultiplayer; i++) {

            player[i] = null;

        }

    }

    public bool checkIfSpecificPlayerIsDead(int playerNum) {

        if(playerNum < numOfPlayers) {

            if(player[playerNum].getIsDead()) {
                return true;
            }

        }

        return false;

    }

    public int getWinner() {
        return playerNumWinner;
    }

}
