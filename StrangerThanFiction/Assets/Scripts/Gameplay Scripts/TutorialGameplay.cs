using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialGameplay : MonoBehaviour
{
    private GameManager gameManager;

    [HeaderAttribute("Input Actions")]
    [SerializeField] private InputActionReference click;

    [TextArea]
    [SerializeField] private string[] OnGameStartPrompts;
    [SerializeField] private string[] OnRound1StartPrompts;
    [SerializeField] private string[] OnFirstCardPlayedPrompts;
    [SerializeField] private string[] OnFirstUnitPlayedPrompts;
    [SerializeField] private string[] OnFirstSpellPlayedPrompts;

    private bool _nextPromptRequested = false;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();

        GameManager.OnGameStart += OnGameStart;
        //gameManager.player1.OnCardPlayed += ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClosePrompt(InputAction.CallbackContext context)
    {
        gameManager.uiManager.SetPrompt(false);
        click.action.performed -= ClosePrompt;
        Debug.Log("Prompt Closed");
    }

    private void NextPromptRequested(InputAction.CallbackContext context)
    { 
        _nextPromptRequested = true;
        click.action.performed -= NextPromptRequested;
        Debug.Log("Next Prompt Requested");
    }

    private async Task OnGameStart()
    {
        Debug.Log("Game Start called in tutorial");

        for (int i = 0; i < OnGameStartPrompts.Length; i++)
        {
            gameManager.uiManager.SetPrompt(true, OnGameStartPrompts[i]);

            if (i < OnGameStartPrompts.Length - 1)
            {
                click.action.performed += NextPromptRequested;
                
                do
                {
                    await Task.Yield();
                } while (!_nextPromptRequested);

                _nextPromptRequested = false;
            }
            else
            {
                click.action.performed += ClosePrompt;

            }

        }

        GameManager.OnGameStart -= OnGameStart;

    }



}
