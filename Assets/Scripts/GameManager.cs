using UnityEngine;

public class GameManager : MonoBehaviour {

    private int[] playerAssign = new int[Global.maxPlayersOverall];

    private Global.sceneMenuIndex currentScene;
    private Global.gameModes currrentGameMode;

    private void Awake() {

        if (Global.gameManager == null) { Global.gameManager = this; }
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        for(int i = 0; i < Global.maxPlayersOverall; i++) {

            playerAssign[i] = 0;

        }

        currentScene = Global.sceneMenuIndex.nullScene;
        currrentGameMode = Global.gameModes.notInGameMode;

    }

    public void assignPlayers(int playerNumber) {

        for (int i = 0; i < Global.maxPlayersOverall; i++) {

            if (playerAssign[i] == 0) {
                playerAssign[i] = playerNumber;
                break;
            }

        }

    }

    public void resetsAssignedPlayers() {

        for (int i = 0; i < Global.maxPlayersOverall; i++) {

            playerAssign[i] = 0;

        }

    }

    public int getTotalPlayersAssigned() {

        int counter = 0;

        for(int i = 0; i < Global.maxPlayersOverall; i++) {

            if(playerAssign[i] != 0) {
                counter++;
            }

        }

        return counter;

    }

    public int getAssignedPlayer(int playerNum) {
        return playerAssign[playerNum];
    }

    public void setCurrentScene(int scene) {
        currentScene = (Global.sceneMenuIndex)scene;
    }

    public void setCurrrentGameMode(Global.gameModes gameMode) {
        currrentGameMode = gameMode;
    }

    public Global.sceneMenuIndex getCurrentScene() {
        return currentScene;
    }

    public Global.gameModes getCurrrentGameMode() {
        return currrentGameMode;
    }
    
}
