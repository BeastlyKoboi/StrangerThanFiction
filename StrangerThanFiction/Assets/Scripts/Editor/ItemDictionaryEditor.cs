using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDictionary))]
public class ItemDictionaryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ItemDictionary itemDictionary = (ItemDictionary)target;

        if (GUILayout.Button("Update Item Dictionary"))
        {
            UpdateItemDictionary(itemDictionary);
        }
    }

    private void UpdateItemDictionary(ItemDictionary itemDictionary)
    {
        // Clear the existing entries
        itemDictionary.itemEntries.Clear();

        // Find all instances of CardData (or replace with your ScriptableObject type)
        string[] guids = AssetDatabase.FindAssets("t:ItemInfo");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemInfo itemInfo = AssetDatabase.LoadAssetAtPath<ItemInfo>(path);

            if (itemInfo != null)
            {
                ItemDictionary.ItemEntry entry = new ItemDictionary.ItemEntry
                {
                    itemName = itemInfo.name, 
                    itemData = itemInfo
                };

                itemDictionary.itemEntries.Add(entry);
            }
        }

        // Mark the object as dirty to save the changes
        EditorUtility.SetDirty(itemDictionary);
        AssetDatabase.SaveAssets();
        Debug.Log("Card Dictionary updated successfully!");
    }
}