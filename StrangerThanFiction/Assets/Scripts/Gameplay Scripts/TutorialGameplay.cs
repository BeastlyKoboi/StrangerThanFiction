using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialGameplay : MonoBehaviour
{
    private CombatManager combatManager;

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
        combatManager = GetComponent<CombatManager>();

        combatManager.OnGameStart.AddListener(OnGameStart);
        combatManager.OnRoundStart.AddListener(OnRound1Start);
        combatManager.player1.OnBeforeCardPlayed.AddListener(OnFirstCardPlayed);
        combatManager.player1.OnBeforeCardPlayed.AddListener(OnFirstUnitPlayed);
        combatManager.player1.OnBeforeCardPlayed.AddListener(OnFirstSpellPlayed);
        combatManager.OnRoundEnd.AddListener(OnRound1End);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClosePrompt(InputAction.CallbackContext context)
    {
        combatManager.uiManager.SetPrompt(false);
        _promptClosed = true;
        click.action.performed -= ClosePrompt;
    }

    private void NextPromptRequested(InputAction.CallbackContext context)
    { 
        _promptClosed = true;
        click.action.performed -= NextPromptRequested;
    }

    private async UniTask CyclePrompts(string[] prompts)
    {
        for (int i = 0; i < prompts.Length; i++)
        {
            combatManager.uiManager.SetPrompt(true, prompts[i]);

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
                await UniTask.Yield();
            } while (!_promptClosed);

            _promptClosed = false;
        }
    }

    private async UniTask OnGameStart(CombatEnterState combatEnterState)
    {
        await CyclePrompts(OnGameStartPrompts);

        combatManager.OnGameStart.RemoveListener(OnGameStart);
    }

    private async UniTask OnRound1Start()
    {
        await CyclePrompts(OnRound1StartPrompts);

        combatManager.OnRoundStart.RemoveListener(OnRound1Start);
    }

    private async UniTask OnRound1End()
    {
        await CyclePrompts(OnRound1EndPrompts);

        combatManager.OnRoundEnd.RemoveListener(OnRound1End);
    }

    private async UniTask OnFirstCardPlayed(CardPlayState playState)
    {
        await CyclePrompts(OnFirstCardPlayedPrompts);

        combatManager.player1.OnBeforeCardPlayed.RemoveListener(OnFirstCardPlayed);
    }

    private async UniTask OnFirstUnitPlayed(CardPlayState playState)
    {
        if (playState.card.Type != CardType.Unit) return;

        await CyclePrompts(OnFirstUnitPlayedPrompts);

        combatManager.player1.OnBeforeCardPlayed.RemoveListener(OnFirstUnitPlayed);
    }

    private async UniTask OnFirstSpellPlayed(CardPlayState playState)
    {
        if (playState.card.Type != CardType.Spell) return;

        await CyclePrompts(OnFirstSpellPlayedPrompts);

        combatManager.player1.OnBeforeCardPlayed.RemoveListener(OnFirstSpellPlayed);
    }


}
