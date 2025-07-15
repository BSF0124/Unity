using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CardOwnership
{
    public int cardID;
    public int quantity;

    public CardOwnership(int cardID, int quantity)
    {
        this.cardID = cardID;
        this.quantity = quantity;
    }
}

[System.Serializable]
public class Stage
{
    public int stageID;
    public bool stageClear;

    public Stage(int stageID, bool stageClear)
    {
        this.stageID = stageID;
        this.stageClear = stageClear;
    }
}

[System.Serializable]
public class PlayerData
{
    public List<CardOwnership> cardOwnerships = new List<CardOwnership>();
    public List<Stage> stage = new List<Stage>();
    public int cardPack;
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;
    public PlayerData playerData = new PlayerData();
    
    private string jsonFilePath;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        jsonFilePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        LoadData();
    }

    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(jsonFilePath, jsonData);
    }

    public void LoadData()
    {
        if(File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            InitializeData();
        }
    }

    private void InitializeData()
    {
        playerData = new PlayerData();
        
        for(int i=0; i<30; i++)
        {
            playerData.cardOwnerships.Add(new CardOwnership(i, 0));
        }

        for(int i=0; i<=20; i++)
        {
            playerData.stage.Add(new Stage(i, false));
        }
        playerData.cardPack = 0;

        SaveData();
    }

    public void AddCard(int cardID, int quantity)
    {
        if(cardID < 0 || cardID >= 30)
        {
            Debug.LogWarning("Invalid card ID");
            return;
        }

        CardOwnership existingCard = playerData.cardOwnerships.Find(card => card.cardID ==  cardID);
        if(existingCard != null)
        {
            existingCard.quantity = existingCard.quantity + quantity < 0? 0 : existingCard.quantity + quantity;
            SaveData();
        }
        else
        {
            Debug.LogWarning("Invalid card ID");
        }
    }

    public void ResetData()
    {
        InitializeData();
        SaveData();
    }
}
