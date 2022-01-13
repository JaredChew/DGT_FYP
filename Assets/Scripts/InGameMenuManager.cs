using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour {

    private int Width;
    private int Height;

    private bool windowFullscreenState;
    private bool speakerState;

    public AudioMixer audioMixer;
    public Slider loadingSlider;
    public Animator animatorSpeakerState;

    private void Start() {

        //For game starting won't pause
        Time.timeScale = 1f;

        speakerState = true;
        animatorSpeakerState.SetBool("SpeakerState", speakerState);

    }

    public void onClickSpeakerButton() {

        speakerState = !speakerState;
        animatorSpeakerState.SetBool( "SpeakerState", speakerState );

    }


    public void saveGame()
    {

        Debug.Log("Saved!");

    }

    public void resume() {

        Time.timeScale = 1f;

    }

    public void replay() {

        if (FindObjectOfType<GameManager>().getCurrrentGameMode() == Global.gameModes.multiPlayer) {
            StartCoroutine(LoadNewScene(SceneManager.GetActiveScene().buildIndex));
        }
        else if(FindObjectOfType<GameManager>().getCurrrentGameMode() == Global.gameModes.survival) {
            StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.survival));
        }

    }

    public void ReturnToMenu() {

        StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.mainMenu));

    }

    public void playMultiplayer(int levelIndex) {

        switch (levelIndex) {

            case 1:
                StartCoroutine(LoadNewScene((int)Global.sceneMenuIndex.multiPlayer_L1));
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
                Debug.Log("Map does not exist!");
                break;

        }

    }

    private IEnumerator LoadNewScene(int sceneIndex) {

        int convertProgress = 0;
        int aimProgress = 0;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f) {

            aimProgress = (int)asyncOperation.progress * 100;

            while (convertProgress < aimProgress) {

                ++convertProgress;
                setLoadingSliderPercentage(convertProgress);

                yield return new WaitForEndOfFrame();

            }

        }

        aimProgress = 100;

        while (convertProgress < aimProgress) {

            ++convertProgress;
            setLoadingSliderPercentage(convertProgress);

            yield return new WaitForEndOfFrame();

        }

        asyncOperation.allowSceneActivation = true;

    }

    private void setLoadingSliderPercentage(int progress) {

        loadingSlider.value = progress;

    }

    public void Exit() {

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

    public void setWindowResolution(int resolutionIndex) {

        switch (resolutionIndex) {

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

        Screen.SetResolution(Width, Height, windowFullscreenState);

    }

    public void setFullscreen(bool fullscreen) {

        windowFullscreenState = fullscreen;
        Screen.SetResolution(Width, Height, windowFullscreenState);

    }

    public void setVolume(float volume) {

        audioMixer.SetFloat("Volume", volume);

    }

    public void setGraphicsQuality(int graphicsQualityIndex) {

        QualitySettings.SetQualityLevel(graphicsQualityIndex);

    }

}
