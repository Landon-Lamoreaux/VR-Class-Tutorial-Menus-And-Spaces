using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEvents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // SceneManager.UnloadSceneAsync("SandBox");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DefaultSceneChange()
    {
        PlayerPrefs.SetInt("count", 5);
        PlayerPrefs.SetInt("sand", 2000);
        // Load the scene by name.
        AsyncOperation op = SceneManager.LoadSceneAsync("SandBox", LoadSceneMode.Additive);
        op.completed += (AsyncOperation o) =>
        {
            // Now that the scene has started loading, find it by name.
            Scene scene = SceneManager.GetSceneByName("SandBox");
            SceneManager.UnloadSceneAsync("Main Scene");
            SceneManager.SetActiveScene(scene);
        };
    }
    public void EasyModeSceneChange()
    {
        PlayerPrefs.SetInt("count", 15);
        PlayerPrefs.SetInt("sand", 0);

        // Load the scene by name.
        AsyncOperation op = SceneManager.LoadSceneAsync("SandBox", LoadSceneMode.Additive);
        op.completed += (AsyncOperation o) =>
        {
            // Now that the scene has started loading, find it by name.
            Scene scene = SceneManager.GetSceneByName("SandBox");
            SceneManager.UnloadSceneAsync("Main Scene");
            SceneManager.SetActiveScene(scene);
        };
    }
}
