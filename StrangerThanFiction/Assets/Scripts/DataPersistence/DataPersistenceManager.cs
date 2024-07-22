using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// Taken from a video tutorial \
/// https://youtu.be/aUi9aijvpgs?si=tUkLEUV9WNJRPzs-
/// Does not currently save gamedata when transitioning between scenes.
/// Need to call save game before transitioning to a new scene.
/// </summary>
public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "test";


    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            // Debug.LogError("Found more than one Data Persistence Manager in the scene.");
            Destroy(this.gameObject);
            return; 
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
        }
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    
    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void NewGame(string profile)
    {
        this.gameData = new GameData();

        List<DeckEntry> defaultCardList = new List<DeckEntry>();

        switch (profile)
        {
            case "LittleRed": 
                break;
            case "Pinocchio":
                defaultCardList.Add(new DeckEntry("Pinocchio", 2));
                defaultCardList.Add(new DeckEntry("Donkey", 2));
                defaultCardList.Add(new DeckEntry("TheTalkingCricket", 2));
                defaultCardList.Add(new DeckEntry("Candlewick", 2));
                defaultCardList.Add(new DeckEntry("GrowthSpurt", 2));


                break;
            case "HumptyDumpty":
                break;
        }

        gameData.player1Deck = new DeckInventory();
        gameData.player1Deck.SetDeckEntries(defaultCardList);

    }

    public void LoadGame()
    {
        if (disableDataPersistence) return;

        // Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(selectedProfileId);

        // if no data can be loaded and debugging, initialize it to a new game.
        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        // if no data can be loaded, do nothing.
        if (this.gameData == null)
        {
            Debug.Log("No data was found.");
            return;
        }

        // push the Loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if (disableDataPersistence) return;

        // if no data can be loaded, do nothing.
        if (this.gameData == null)
        {
            Debug.Log("No data was found.");
            return;
        }

        // pass the data to other scripts so they can update it 
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // Save that data to a file using the daata handler
        dataHandler.Save(gameData, selectedProfileId);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return this.gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

}
