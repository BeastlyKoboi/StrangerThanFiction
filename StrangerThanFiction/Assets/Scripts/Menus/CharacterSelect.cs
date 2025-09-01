using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour, IDataPersistence
{
    private GameData gameData;
    [SerializeField] private CharacterProfile littleRedProfile;
    [SerializeField] private CharacterProfile pinocchioProfile;
    [SerializeField] private CharacterProfile humptyDumptyProfile;

    public void OnContinueClicked(CharacterProfile characterProfile)
    {
        gameData.selectedFaction = characterProfile.GetFaction();
    }

    public void OnNewGameClicked(CharacterProfile characterProfile)
    {
        gameData.selectedFaction = characterProfile.GetFaction();
        gameData.SetRunData(characterProfile.GetFaction(), new RunData());

        RunData runData = gameData.GetRunData();
        runData.runHasStarted = true;
        runData.player1Deck = new DeckInventory();
        runData.player1Deck.SetDeckEntries(characterProfile.GetCharacterDeck());
        if (characterProfile.GetSeed() != null) 
            runData.currentSeed = characterProfile.GetSeed();
        else 
            runData.currentSeed = System.DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    public void LoadData(GameData data)
    {
        gameData = data;

        if (littleRedProfile)
            littleRedProfile.SetData(gameData.littleRedRunData);
        if (pinocchioProfile)
            pinocchioProfile.SetData(gameData.pinocchioRunData);
        if (humptyDumptyProfile)
            humptyDumptyProfile.SetData(gameData.humptyDumptyRunData);
    }

    public void SaveData(GameData data)
    {
        
    }
}
