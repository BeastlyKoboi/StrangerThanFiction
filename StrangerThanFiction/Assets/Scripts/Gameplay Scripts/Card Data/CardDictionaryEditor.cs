using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(CardDictionary))]
public class CardDictionaryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CardDictionary cardDictionary = (CardDictionary)target;

        if (GUILayout.Button("Update Card Dictionary"))
        {
            UpdateCardDictionary(cardDictionary);
        }
    }

    private void UpdateCardDictionary(CardDictionary cardDictionary)
    {
        // Clear the existing entries
        cardDictionary.cardEntries.Clear();

        // Find all instances of CardData (or replace with your ScriptableObject type)
        string[] guids = AssetDatabase.FindAssets("t:CardInfo");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CardInfo cardData = AssetDatabase.LoadAssetAtPath<CardInfo>(path);

            if (cardData != null)
            {
                CardDictionary.CardEntry entry = new CardDictionary.CardEntry
                {
                    cardName = cardData.name, // Assuming the card's name is its object name
                    cardData = cardData
                };

                cardDictionary.cardEntries.Add(entry);
            }
        }

        // Mark the object as dirty to save the changes
        EditorUtility.SetDirty(cardDictionary);
        AssetDatabase.SaveAssets();
        Debug.Log("Card Dictionary updated successfully!");
    }
}