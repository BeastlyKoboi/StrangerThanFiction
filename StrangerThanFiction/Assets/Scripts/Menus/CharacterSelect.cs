using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    private CharacterProfile[] characterProfiles;

    private void Awake()
    {
        // Get all the character profiles in the scene
        characterProfiles = GetComponentsInChildren<CharacterProfile>();
    }

    private void Start()
    {
        ActivateMenu();
    }

    public void ActivateMenu()
    {
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        foreach (CharacterProfile characterProfile in characterProfiles)
        {
            profilesGameData.TryGetValue(characterProfile.GetProfileId(), 
                out GameData profileData);
            characterProfile.SetData(profileData);
        }
    }

    public void OnContinueClicked(CharacterProfile characterProfile)
    {
        DataPersistenceManager.instance.ChangeSelectedProfileId(characterProfile.GetProfileId());
        if (!characterProfile.hasData)
        {
            DataPersistenceManager.instance.NewGame(characterProfile.GetProfileId());
        }

    }

    public void OnNewGameClicked(CharacterProfile characterProfile)
    {
        DataPersistenceManager.instance.ChangeSelectedProfileId(characterProfile.GetProfileId());
        DataPersistenceManager.instance.NewGame(characterProfile.GetProfileId());
    }
}
