using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryPage : MonoBehaviour, IDataPersistence
{
    private GameData gameData;
    [SerializeField] private GameObject paragraphsParentObj;
    [SerializeField] private GameObject paragraphPrefab;

    public void LoadData(GameData data)
    {
        gameData = data;
        UpdateStory();  
    }

    public void SaveData(GameData data)
    {
        
    }

    public void AddToStorybook(string newText)
    {
        gameData.storyParagraphs.Add(newText);
        UpdateStory();
    }

    public void UpdateStory()
    {
        // Clear all existing paragraphs
        foreach (Transform child in paragraphsParentObj.transform)
        {
            Destroy(child.gameObject);
        }

        // Add new paragraphs
        foreach (string paragraph in gameData.storyParagraphs)
        {
            GameObject newParagraph = Instantiate(paragraphPrefab, paragraphsParentObj.transform);
            newParagraph.GetComponent<TMPro.TextMeshProUGUI>().text = paragraph;
        }

    }
}