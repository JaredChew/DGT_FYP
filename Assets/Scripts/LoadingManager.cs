using System.Collections;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour {

    public Slider loadingSlider;
    private AsyncOperation asyncOperation;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadingScreen(int sceneIndex)
    {
        LoadNewScene(sceneIndex);
    }

    IEnumerator LoadNewScene( int sceneIndex )
    {
        //For waiting
        //yield return new WaitForSeconds(2);

        //For disallow the AsyncOperation automatic change the scene when loading finished
        asyncOperation.allowSceneActivation = false;

        //asyncOperation = Application.LoadLevelAsync(sceneIndex);
        asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while (asyncOperation.isDone == false) {

            loadingSlider.value = asyncOperation.progress;

            if (asyncOperation.progress == 0.9f) {

                loadingSlider.value = 1f;
                asyncOperation.allowSceneActivation = true;

            }

            yield return null;

        }

    }

}
