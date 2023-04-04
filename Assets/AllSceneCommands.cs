using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AllSceneCommands : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerInput filter = GameObject.FindObjectOfType<PlayerInput>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<PlayerInput>();
            Preset inputSettings = Resources.Load<Preset>("PlayerInputGrab");
            inputSettings.ApplyTo(filter);
            filter.ActivateInput();
        }
        filter.actions["MainMenu"].started += OnMainMenu;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMainMenu(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;
        AsyncOperation op = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
        op.completed += (AsyncOperation o) =>
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(0);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneManager.SetActiveScene(scene);
        };
    }

    public void Awake()
    {
        // First load, load main menu.
        if (SceneManager.GetActiveScene().name == "Players" && SceneManager.sceneCount == 1)
        {
            // Load the scene by index.
            AsyncOperation op = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
            op.completed += (AsyncOperation o) =>
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
            };
        }
    }
}
