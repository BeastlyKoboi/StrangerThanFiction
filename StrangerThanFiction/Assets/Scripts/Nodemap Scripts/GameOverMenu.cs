using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    private SceneLoader sceneLoader;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ToggleGameOverMenu(bool isVisible)
    {
        if (isVisible) 
        {
            canvasGroup.alpha = 1; 
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
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
        sceneLoader.LoadScene(GameScenes.HomeMenu);
    }

    public void LoadCharacterSelect()
    {
        if (sceneLoader == null)
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
        }
        sceneLoader.LoadScene(GameScenes.CharacterSelect);
    }
}
