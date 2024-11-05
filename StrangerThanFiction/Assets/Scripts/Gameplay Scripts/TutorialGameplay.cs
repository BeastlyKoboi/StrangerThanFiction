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

    
    [SerializeField][TextArea] private string[] OnGameStartPrompts;
    [SerializeField][TextArea] private string[] OnRound1StartPrompts;
    [SerializeField][TextArea] private string[] OnRound1EndPrompts;
    [SerializeField][TextArea] private string[] OnFirstCardPlayedPrompts;
    [SerializeField][TextArea] private string[] OnFirstUnitPlayedPrompts;
    [SerializeField][TextArea] private string[] OnFirstSpellPlayedPrompts;

    private bool _promptClosed = false;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();

        GameManager.OnGameStart += OnGameStart;
        GameManager.OnRoundStart += OnRound1Start;
        gameManager.player1.OnCardPlayed += OnFirstCardPlayed;
        gameManager.player1.OnCardPlayed += OnFirstUnitPlayed;
        gameManager.player1.OnCardPlayed += OnFirstSpellPlayed;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClosePrompt(InputAction.CallbackContext context)
    {
        gameManager.uiManager.SetPrompt(false);
        _promptClosed = true;
        click.action.performed -= ClosePrompt;
    }

    private void NextPromptRequested(InputAction.CallbackContext context)
    { 
        _promptClosed = true;
        click.action.performed -= NextPromptRequested;
    }

    private async Task CyclePrompts(string[] prompts)
    {
        for (int i = 0; i < prompts.Length; i++)
        {
            gameManager.uiManager.SetPrompt(true, prompts[i]);

            if (i < prompts.Length - 1)
            {
                click.action.performed += NextPromptRequested;
            }
            else
            {
                click.action.performed += ClosePrompt;
            }

            do
            {
                await Task.Yield();
            } while (!_promptClosed);

            _promptClosed = false;
        }
    }

    private async Task OnGameStart()
    {
        await CyclePrompts(OnGameStartPrompts);

        GameManager.OnGameStart -= OnGameStart;
    }

    private async Task OnRound1Start()
    {
        await CyclePrompts(OnRound1StartPrompts);

        GameManager.OnRoundStart -= OnRound1Start;
    }

    private async Task OnRound1End()
    {
        await CyclePrompts(OnRound1EndPrompts);

        GameManager.OnRoundEnd -= OnRound1End;
    }

    private async Task OnFirstCardPlayed(CardPlayState playState)
    {
        await CyclePrompts(OnFirstCardPlayedPrompts);

        gameManager.player1.OnCardPlayed -= OnFirstCardPlayed;
    }

    private async Task OnFirstUnitPlayed(CardPlayState playState)
    {
        if (playState.card.Type != CardType.Unit) return;

        await CyclePrompts(OnFirstUnitPlayedPrompts);

        gameManager.player1.OnCardPlayed -= OnFirstUnitPlayed;
    }

    private async Task OnFirstSpellPlayed(CardPlayState playState)
    {
        if (playState.card.Type != CardType.Spell) return;

        await CyclePrompts(OnFirstSpellPlayedPrompts);

        gameManager.player1.OnCardPlayed -= OnFirstSpellPlayed;
    }


}
