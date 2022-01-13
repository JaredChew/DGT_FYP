using UnityEngine;

using UnityEngine.UI;

public class InGameMenuLink : MonoBehaviour {

    private int Width;
    private int Height;

    private bool windowFullscreenState;
    private bool endGameMenuIsActive;

    public GameObject pauseMenuGameObject;
    public GameObject matchEndMenuGameOPbject;

    public Text endGameText;

    public Button buttonResume;
    public Button buttonEndMenuSelected;

    private void Start()
    {
        endGameMenuIsActive = false;
    }

    

    public void displayPauseMenu() {

        if (!pauseMenuGameObject.activeSelf && endGameMenuIsActive == false) {

            pauseMenuGameObject.SetActive(true);
            buttonResume.Select();
            //matchEndMenuGameOPbject.SetActive(true); //To Debug
            Time.timeScale = 0f;


        }
        else if (pauseMenuGameObject.activeSelf && endGameMenuIsActive == false) {

            pauseMenuGameObject.SetActive(false);
            Time.timeScale = 1f;

        }

    }

    public void showEndMatchMenu() {

        endGameMenuIsActive = true;
        matchEndMenuGameOPbject.SetActive(true);
        buttonEndMenuSelected.Select();
        Time.timeScale = 0f;

    }

    public void setMultiplayerWinnerText(int winnerNumber) {

        endGameText.text = "Player " + winnerNumber + " WON !";

    }

    public void setSurvivalPlayersScoreText(int playerNum, int score) {

        if (string.IsNullOrEmpty(endGameText.text)) {
            endGameText.text = "Player " + playerNum + " Score: " + score;
        }
        else {
            endGameText.text = "\nPlayer " + playerNum + " Score: " + score;
        }

    }
    /*
    //For adding the value of Kill
    public void addKill( int playerIndex, int killPoint = 1 )
    {
        kill[playerIndex] += killPoint;
    }

    //For adding the value of Scope
    public void addScope( int playerIndex, int scopePoint = 1 )
    {
        scope[playerIndex] += scopePoint;
    }
    */
}
