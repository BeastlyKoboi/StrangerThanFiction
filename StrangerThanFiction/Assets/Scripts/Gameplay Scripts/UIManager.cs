using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI of the gameplay
/// </summary>
public class UIManager : MonoBehaviour
{
    [HeaderAttribute("Game State")]
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private BoardManager board;

    [HeaderAttribute("Animators")]
    [SerializeField] private Animator RoundPopup;
    [SerializeField] private Animator GameOverPopup;

    [HeaderAttribute("Player 1")]
    [SerializeField] private Player player1; // The human player
    [SerializeField] private TextMeshProUGUI deckLabelPlayer1;
    [SerializeField] private TextMeshProUGUI discardLabelPlayer1;
    [SerializeField] private TextMeshProUGUI manaPlayer1;
    [SerializeField] private TextMeshProUGUI powerPlayer1;

    [HeaderAttribute("Player 2")]
    [SerializeField] private Player player2; // The AI eventually 
    [SerializeField] private TextMeshProUGUI deckLabelPlayer2;
    [SerializeField] private TextMeshProUGUI discardLabelPlayer2;
    [SerializeField] private TextMeshProUGUI manaPlayer2;
    [SerializeField] private TextMeshProUGUI powerPlayer2;

    


    [HeaderAttribute("Paused Menu")]
    [SerializeField] private GameObject PausedMenu;

    [HeaderAttribute("Game Over UI")]
    [SerializeField] private TextMeshProUGUI playerTotalPowerCount;
    [SerializeField] private TextMeshProUGUI enemyTotalPowerCount;
    [SerializeField] private TextMeshProUGUI overkillText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI gainedDeusText;

    [HeaderAttribute("Buttons")]
    [SerializeField] private Button rightMiddleBtn;
    [SerializeField] private TextMeshProUGUI rightMiddleBtnText;
    [SerializeField] private Animator rightMiddleBtnAnimator;
    [SerializeField] private bool rightMiddleBtnIsVisible = false;

    [HeaderAttribute("Prompt")]
    [SerializeField] private GameObject prompt;
    [SerializeField] private TextMeshProUGUI promptText;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Activates the Round Start Popup
    /// </summary>
    /// <param name="roundNum"></param>
    public void RoundStart(int roundNum, int maxRounds)
    {
        if (RoundPopup == null) return;
        RoundPopup.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"Chapter {roundNum}";
        RoundPopup.SetTrigger("Popup");
    }

    //public async void GameStart()
    //{


    //}

    /// <summary>
    /// Activates the Game over Popup and show the results.
    /// </summary>
    public void GameOver(GameOverState gameOverState)
    {
        int playerTotalPower = board.GetTotalPower(combatManager.player1);
        int enemyTotalPower = board.GetTotalPower(combatManager.player2);

        playerTotalPowerCount.text = playerTotalPower.ToString();
        enemyTotalPowerCount.text = enemyTotalPower.ToString();

        if (gameOverState.hasPlayerWon)
        {
            resultText.text = "You Win";
            overkillText.text = $"Binding Overkill: {(gameOverState.bindingDamage - gameOverState.bindingPower).ToString()}";
            gainedDeusText.text = $"Gained Deus: {gameOverState.unusedInk}";
        }
        else
        {
            resultText.text = "You Lose";
            overkillText.gameObject.SetActive(false);
        }

        GameOverPopup.gameObject.SetActive(true);
        GameOverPopup.SetTrigger("GameOver");
    }

    /// <summary>
    /// Updates the deck count of the player
    /// </summary>
    /// <param name="player"></param>
    public void UpdateDeck(Player player)
    {
        if (player == player1)
            deckLabelPlayer1.text = $"{player1.Deck.Count}";
        else if (player == player2)
            deckLabelPlayer2.text = $"{player2.Deck.Count}";
    }

    /// <summary>
    /// Updates the discard count of the player
    /// </summary>
    /// <param name="player"></param>
    public void UpdateDiscard(Player player)
    {
        if (player == player1)
            discardLabelPlayer1.text = $"{player1.Discard.Count}";
        else if (player == player2)
            discardLabelPlayer2.text = $"{player2.Discard.Count}";
    }

    /// <summary>
    /// Updates the mana of the player
    /// </summary>
    public void UpdateMana(Player player)
    {
        if (player == player1)
            manaPlayer1.text = $"{player1.CurrentMana}/{player1.MaxMana}";
        else if (player == player2)
            manaPlayer2.text = $"{player2.CurrentMana}/{player2.MaxMana}";

    }

    /// <summary>
    /// Updates the total power of the players
    /// </summary>
    public void UpdateTotalPower()
    {
        int totalPower = board.GetTotalPower(player1);
        powerPlayer1.text = totalPower.ToString();

        totalPower = board.GetTotalPower(player2);
        powerPlayer2.text = totalPower.ToString();
    }

    public void TogglePausedMenu(bool isActive)
    {
        PausedMenu.SetActive(isActive);
    }

    public void SetRightMiddleButton(bool visible, string text, Action onClick)
    {
        if (visible && rightMiddleBtnIsVisible != visible)
        {
            rightMiddleBtnAnimator.SetBool("Visible", true);
            rightMiddleBtnIsVisible = visible;
        }
        else if (!visible && rightMiddleBtnIsVisible != visible)
        {
            rightMiddleBtnAnimator.SetBool("Visible", false);
            rightMiddleBtnIsVisible = visible;
        }

        rightMiddleBtnText.text = text;
        rightMiddleBtn.onClick.RemoveAllListeners();
        rightMiddleBtn.onClick.AddListener(() => onClick());
    }

    public void SetPrompt(bool isActive, string text = "Default")
    {
        prompt.SetActive(isActive);
        promptText.text = text;
    }
}
