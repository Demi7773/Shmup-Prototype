using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgmt : MonoBehaviour
{

    private GameManager _gm;

    //private Scene _managers;
    private Scene _scene0;
    private Scene _mainMenuScene;
    private Scene _currentLevelScene;


    private List<Scene> _loadedScenes = new List<Scene>();





    public void Init(GameManager gm)
    {
        _gm = gm;
        _scene0 = SceneManager.GetSceneAt(0);
        _loadedScenes.Add(_scene0);
        StartCoroutine(LoadMainMenu());
    }






    private IEnumerator LoadMainMenu()
    {
        yield return null;

        int index = SceneManager.sceneCount;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneNameStrings.MainMenuName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene spawnedScene = SceneManager.GetSceneAt(index);
        _mainMenuScene = spawnedScene;
        _loadedScenes.Add(_mainMenuScene);
        //Debug.Log("Scene Loaded Test, stored reference: " + spawnedScene.name + ". If this is incorrect, index probably needs to be +1");
    }



    public void LoadNewLevel(string sceneName)
    {
        //Debug.Log("Call from GM to Load Scene: " + sceneName);
        StartCoroutine(AsyncLoadSceneByName(sceneName));
    }
    private IEnumerator AsyncLoadSceneByName(string sceneName)
    {
        UIEvents.SetLoadBarProgress(0.0f);
        UIEvents.ChangeUIState(UIManager.UIState.Loading);
        //Debug.Log("Start of LoadScene Sequence: " + sceneName);

        yield return null;

        int index = SceneManager.sceneCount;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            UIEvents.SetLoadBarProgress(asyncLoad.progress);
            //Debug.Log(sceneName + " loading: " + asyncLoad.progress);
            yield return null;
        }

        Scene spawnedScene = SceneManager.GetSceneAt(index);
        _currentLevelScene = spawnedScene;
        //Debug.Log("Scene Loaded Test, stored reference: " + spawnedScene.name + ". If this is incorrect, index probably needs to be +1");
        
    }


    public void UnloadCurrentLevel(bool wasLevelCompleted)
    {
        //Debug.Log("Start of Unload Sequence in SceneMgmt: " + _currentLevelScene.name);
        StartCoroutine(AsyncUnloadCurrentLevel(wasLevelCompleted));
    }

    private IEnumerator AsyncUnloadCurrentLevel(bool wasLevelCompleted)
    {
        yield return null;

        AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(_currentLevelScene);
        while (!asyncOp.isDone)
        {
            //Debug.Log("Unloading Current Level progress: " + asyncOp.progress);
            yield return null;
        }

        Debug.Log("AsyncUnloadCurrentLevel Complete");
        
        _gm.OnLevelUnloaded(wasLevelCompleted);
    }

}
