using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    //Take request Character Images from physic main menu
    public GameObject[] selectorChracterImage_MP = new GameObject[4];
    public GameObject[] selectorChracterImage_SV = new GameObject[4];

    public AudioMixer audioMixer;
    public Slider loadingSlider;

    int playersPlaying_Debug = 0;

    private int Width;
    private int Height;
    private int quality;

    private bool mpPlayersJoinMenu;
    private bool svPlayersJoinMenu;

    private bool windowFullscreenState;

    public GameObject btnMultiplayer;

    private void Awake() {

        windowFullscreenState = false;
        setWindowResolution( (int)Global.windowResolution.win640x480 );

    }

    private void Start() {

        //For game starting won't pause
        Time.timeScale = 1f;

        mpPlayersJoinMenu = false;
        svPlayersJoinMenu = false;

        Global.gameManager.resetsAssignedPlayers();
        
        Global.audioManager.playMusic(Global.music_Test); //Testing

        btnMultiplayer.GetComponent<Button>().Select();

        FindObjectOfType<AudioManager>().playMusic("Menu_BGM");

    }

    private void Update() {
        
        if(mpPlayersJoinMenu) {

            if (Global.gameManager.getTotalPlayersAssigned() >= Global.minPlayersInMultiplayer) {
                transform.Find("Menus/MP_PlayerJoin/ButtonStart").GetComponent<Button>().gameObject.SetActive(true);
            }
            else {
                transform.Find("Menus/MP_PlayerJoin/ButtonStart").GetComponent<Button>().gameObject.SetActive(false);
            }

            playerJoin();

        }
        else if(svPlayersJoinMenu) {

            //This section is done quickly, please improve it if you have any idea

            if (Global.gameManager.getTotalPlayersAssigned() >= Global.minPlayersInSurvival) {
                transform.Find("Menus/SV_PlayerJoin/ButtonStart").GetComponent<Button>().gameObject.SetActive(true);
            }
            else {
                transform.Find("Menus/SV_PlayerJoin/ButtonStart").GetComponent<Button>().gameObject.SetActive(false);
            }

            playerJoin();

        }

    }

    //Check and detect which Join Menu character image request to set active
    private void detectWhichJoinMenuAndSetCharacterImageActive() {

        if (mpPlayersJoinMenu) {

            selectorChracterImage_MP[Global.gameManager.getTotalPlayersAssigned()].SetActive(true);
        }
        else if (svPlayersJoinMenu) {

            selectorChracterImage_SV[Global.gameManager.getTotalPlayersAssigned()].SetActive(true);
        }
        
    }

    private void playerJoin() {
        
        for(int i = 1; i > Global.maxControllers; i++) {

            if (Input.GetButton(Global.inputAxes_SelectDecline + i)) {

                Global.gameManager.assignPlayers(i);

                //Check which character image request to set active
                detectWhichJoinMenuAndSetCharacterImageActive();

                break;
            }

        }

        /* //For backup
        if (Input.GetButton(Global.inputAxes_SelectDecline + 1)) {
            Global.gameManager.assignPlayers(1);
        }
        else if (Input.GetButton(Global.inputAxes_SelectDecline + 2)) {
            Global.gameManager.assignPlayers(2);
        }
        else if (Input.GetButton(Global.inputAxes_SelectDecline + 3)) {
            Global.gameManager.assignPlayers(3);
        }
        else if (Input.GetButton(Global.inputAxes_SelectDecline + 4)) {
            Global.gameManager.assignPlayers(4);
        }
        */


        { //Temp, To debug

            if (Input.GetKeyDown(KeyCode.O) && playersPlaying_Debug <= 4) {

                playersPlaying_Debug++;

                //Check which character image request to set active
                detectWhichJoinMenuAndSetCharacterImageActive();

                Global.gameManager.assignPlayers(playersPlaying_Debug);

                Debug.Log("Num of Players: " + playersPlaying_Debug);

            }
            else if (Input.GetKeyDown(KeyCode.P)) {

                playersPlaying_Debug = 0;

                //Set all Character Image to not active
                resetMultiplayerJoined();
                resetSurvivalJoined();

                Global.gameManager.resetsAssignedPlayers();

                Debug.Log("Num of Players reset");

            }

        } //Temp, To debug

    }

    public void resetPlayersJoined() {

        //For Debug only
        playersPlaying_Debug = 0;

        Global.gameManager.resetsAssignedPlayers();

    }

    public void resetMultiplayerJoined() {

        for (int i = 0; i < selectorChracterImage_MP.Length; i++) {

            selectorChracterImage_MP[i].SetActive(false);

        }

    }

    public void resetSurvivalJoined() {

        for (int i = 0; i < selectorChracterImage_SV.Length; i++)
        {

            selectorChracterImage_SV[i].SetActive(false);

        }

    }

    public void playMultiplayer( int levelIndex ) {

        Global.gameManager.setCurrrentGameMode(Global.gameModes.multiPlayer);

        switch ( levelIndex ){ 

            case 1:
                StartCoroutine( LoadNewScene((int)Global.sceneMenuIndex.multiPlayer_L1) );
                break;

            case 2:
                StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.multiPlayer_L2));
                break;

            case 3:
                StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.multiPlayer_L3));
                break;

            case 4:
                StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.multiPlayer_L4));
                break;

            case 5:
                StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.multiPlayer_L5));
                break;

            default:
                Debug.Log("Sorry this map did not design yet!");
                break;

        }

        //SceneManager.LoadScene( (int)Global.sceneMenuIndex.multiPlayer_L1 );
        //SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex + 1); //SceneManager.LoadScene("Test");

    }

    public void playStory() {

        Global.gameManager.setCurrrentGameMode(Global.gameModes.singlePlayer);

        StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.singlePlayer));

    }

    public void playSurvival() {

        Global.gameManager.setCurrrentGameMode(Global.gameModes.survival);

        StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.survival));

    }
    
    private IEnumerator LoadNewScene(int sceneIndex) {

        int convertProgress = 0;
        int aimProgress = 0;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        asyncOperation.allowSceneActivation = false;

        while ( asyncOperation.progress < 0.9f ) {

            aimProgress = (int)asyncOperation.progress * 100;

            while( convertProgress < aimProgress) {

                ++convertProgress;
                setLoadingSliderPercentage(convertProgress);

                yield return new WaitForEndOfFrame();

            }

        }

        aimProgress = 100;

        while ( convertProgress < aimProgress) {

            ++convertProgress;
            setLoadingSliderPercentage(convertProgress);

            yield return new WaitForEndOfFrame();

        }
        
        asyncOperation.allowSceneActivation = true;

    }

    private void setLoadingSliderPercentage ( float progress ) {

        loadingSlider.value = progress;
        //Debug.Log(progress);

    }
    
    public void ReturnToMenu() {

        SceneManager.LoadScene( (int)Global.sceneMenuIndex.mainMenu );

    }

    public void Exit() {

        Debug.Log("Quit");
        Application.Quit();

    }

    /*
    public void WindowWidth(int w) {
        Width = w;
    }

    public void WindowHeight(int h) {
        Height = h;
    }
    */

    public void setWindowResolution( int resolutionIndex ) {

        switch(resolutionIndex){

            case (int)Global.windowResolution.win640x480:
                Width = 640;
                Height = 480;
                break;

            case (int)Global.windowResolution.win800x600:
                Width = 800;
                Height = 600;
                break;

            case (int)Global.windowResolution.win1280x720:
                Width = 1280;
                Height = 720;
                break;

            default:
                Debug.Log("That window resolution are not in our development!");
                break;

        }

    }

    public void setFullscreen(bool fullscreen) {

        windowFullscreenState = fullscreen;
        

    }

    public void setApplyOption()
    {

        QualitySettings.SetQualityLevel(quality);
        Screen.SetResolution(Width, Height, windowFullscreenState);

    }

    public void setVolume(float volume) {

        audioMixer.SetFloat("Volume", volume);

    }

    public void setGraphicsQuality( int graphicsQualityIndex ) {

        quality = graphicsQualityIndex;

    }
    
    public void setMpPlayersJoinMenu(bool isInStatedMenu) {

        //This section is done quickly, please improve it if you have any idea

        mpPlayersJoinMenu = isInStatedMenu;

        if(!mpPlayersJoinMenu) {
            transform.Find("Menus/MP_PlayerJoin/ButtonStart").GetComponent<Button>().gameObject.SetActive(false);
        }

    }

    public void setSVPlayersJoinMenu(bool isInStatedMenu) {

        //This section is done quickly, please it improve if you have any idea

        svPlayersJoinMenu = isInStatedMenu;

        if (!mpPlayersJoinMenu) {
            transform.Find("Menus/SV_PlayerJoin/ButtonStart").GetComponent<Button>().gameObject.SetActive(false);
        }

    }

    /*
    public void setSelected(GameObject selectRect)
    {

        if(selectRect.GetComponent<Button>() == true)  {

            selectRect.GetComponent<Button>().Select();

        }
        else if(selectRect.GetComponent<Slider>() == true) {

            selectRect.GetComponent<Slider>().Select();

        }
        
    }
    */

}
