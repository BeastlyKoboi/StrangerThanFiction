using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PauseMenu : MonoBehaviour
{
    [HeaderAttribute("Input Actions")]
    [SerializeField] private InputActionReference pause;

    private CanvasGroup canvasGroup;

    private SceneLoader sceneLoader;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        pause.action.performed += TogglePause;

    }
    private void OnDisable()
    {
        pause.action.performed -= TogglePause;
    }

    private void TogglePause(CallbackContext ctx)
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            Time.timeScale = 1;
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void LoadMainMenu()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
        }
        Time.timeScale = 1;
        sceneLoader.LoadScene("Home Menu");
    }

    public void LoadCharacterSelect()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
        }
        Time.timeScale = 1;
        sceneLoader.LoadScene("CharacterSelect");
    }

    /// <summary>
    /// Method to quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
