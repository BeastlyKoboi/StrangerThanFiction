using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlotToggle : MonoBehaviour
{
    [SerializeField] private SaveSlot[] saveSlots;

    private void Start()
    {
        ActivateMenu();
    }

    public void ActivateMenu()
    {
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        foreach (SaveSlot saveSlot in saveSlots)
        {
            profilesGameData.TryGetValue(saveSlot.GetProfileId(),
                out GameData profileData);
            saveSlot.SetData(profileData);
        }

        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlots[0].GetProfileId());
        if (!saveSlots[0].hasData)
        {
            DataPersistenceManager.instance.NewGame(saveSlots[0].GetProfileId());
        }
        saveSlots[0].ToggleSelected(true);
    }

    public void OnContinueClicked(SaveSlot saveSlot)
    {
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
        if (!saveSlot.hasData)
        {
            DataPersistenceManager.instance.NewGame(saveSlot.GetProfileId());
        }
        foreach (SaveSlot slot in saveSlots)
        {
            slot.ToggleSelected(false);
        }
        saveSlot.ToggleSelected(true);
    }
}
