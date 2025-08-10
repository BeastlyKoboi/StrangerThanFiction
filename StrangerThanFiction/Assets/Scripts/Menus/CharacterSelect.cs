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

        runData.boonList.Add("MeekInheritance");
        runData.boonList.Add("Cloudcuckoolander");
        runData.boonList.Add("ImpulsiveTinkering");
    }

    public void LoadData(GameData data)
    {
        gameData = data;

        littleRedProfile.SetData(gameData.littleRedRunData);
        pinocchioProfile.SetData(gameData.pinocchioRunData);
        humptyDumptyProfile.SetData(gameData.humptyDumptyRunData);
    }

    public void SaveData(GameData data)
    {
        
    }
}
