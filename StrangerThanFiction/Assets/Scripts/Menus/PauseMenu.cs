using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PauseMenu : MonoBehaviour, IDataPersistence
{
    [HeaderAttribute("Input Actions")]
    [SerializeField] private InputActionReference pause;

    private CanvasGroup canvasGroup;

    private SceneLoader sceneLoader;

    private float timeScale;
    [SerializeField] private GameObject timeScaleBtnSpeed1;
    [SerializeField] private GameObject timeScaleBtnSpeed2;
    [SerializeField] private GameObject timeScaleBtnSpeed3;

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

    public void TogglePause(CallbackContext ctx)
    {
        TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        if (Time.timeScale == timeScale)
        {
            Time.timeScale = 0;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            Time.timeScale = timeScale;
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
        Time.timeScale = timeScale;
        sceneLoader.LoadScene(GameScenes.HomeMenu);
    }

    public void LoadCharacterSelect()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
        }
        Time.timeScale = timeScale;
        sceneLoader.LoadScene(GameScenes.CharacterSelect);
    }

    public void ChangeTimeScale(float timeScale)
    {
        this.timeScale = timeScale;

        timeScaleBtnSpeed1.transform.localScale = Vector3.one;
        timeScaleBtnSpeed2.transform.localScale = Vector3.one;
        timeScaleBtnSpeed3.transform.localScale = Vector3.one;

        if (timeScale == 1f)
            timeScaleBtnSpeed1.transform.localScale = Vector3.one * 1.2f;
        else if (timeScale == 2f)
            timeScaleBtnSpeed2.transform.localScale = Vector3.one * 1.2f;
        else if (timeScale == 4f)
            timeScaleBtnSpeed3.transform.localScale = Vector3.one * 1.2f;
    }

    /// <summary>
    /// Method to quit the game.
    /// </summary>
    public void QuitGame()
    {
        Time.timeScale = timeScale;
        Application.Quit();
    }

    public void LoadData(GameData data)
    {
        timeScale = data.timeScale;
        Time.timeScale = timeScale;

        ChangeTimeScale(timeScale);
    }

    public void SaveData(GameData data)
    {
        data.timeScale = timeScale;
    }
}
