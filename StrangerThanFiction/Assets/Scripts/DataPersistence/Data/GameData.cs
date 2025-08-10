using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    [Header("Long Term Stats")]
    public int totalRunsCount;

    [Header("Story")]
    public List<string> storyParagraphs;

    public bool hasCompletedTutorial;

    public Faction selectedFaction;

    public RunData littleRedRunData;
    public RunData pinocchioRunData;
    public RunData humptyDumptyRunData;

    [Header("Settings")]
    public float timeScale = 1.0f;
    public bool isMusicEnabled = true;

    public GameData()
    {
        this.totalRunsCount = 0;

        this.storyParagraphs = new List<string>();

        this.hasCompletedTutorial = false;
    }

    public RunData GetRunData()
    {
        switch (selectedFaction)
        {
            case Faction.LittleRed:
                return littleRedRunData;
            case Faction.Pinocchio:
                return pinocchioRunData;
            case Faction.HumptyDumpty:
                return humptyDumptyRunData;
            default:
                Debug.LogError("Selected faction is not set or is invalid.");
                return null;
        }
    }

    public void SetRunData(Faction faction, RunData runData)
    {
        switch (faction)
        {
            case Faction.LittleRed:
                littleRedRunData = runData;
                break;
            case Faction.Pinocchio:
                pinocchioRunData = runData;
                break;
            case Faction.HumptyDumpty:
                humptyDumptyRunData = runData;
                break;
            default:
                Debug.LogError("Selected faction is not set or is invalid.");
                break;
        }

        
    }

}
