using UnityEngine;

public class GameSaveData : IDataSaver
{
    private static GameSaveData instance;
    public static GameSaveData Instance
    {
        get
        {
            instance ??= new GameSaveData(GameData.DefaultOptions());
            return instance;
        }
    }

    private readonly GameData gameData;

    public GameData GameData => gameData;

    public GameSaveData(GameData data)
    {
        gameData = data;
    }

    public void Load()
    {
        Debug.LogWarning("Loading GameSaveData");
        GameData loadedData = (GameData)SaveDataManager.Load(GameData.FileName, typeof(GameData));

        if (loadedData == null)
        {
            Debug.LogError("Failed to load Game Data");
            return;
        }

        gameData.Name = loadedData.Name;
        gameData.Level = loadedData.Level;
        gameData.Position = loadedData.Position;

        Debug.LogWarning($"Loaded Game Data");
        Debug.LogWarning($"Name: {gameData.Name}");
        Debug.LogWarning($"Level: {gameData.Level}");
        Debug.LogWarning($"Pos: {gameData.Position}");
        Debug.LogWarning($"---------------------------------");
    }
    public void Save()
    {
        Debug.LogWarning("Starting save from GameSaveData.cs");
        SaveDataManager.Save(GameData.FileName, gameData);
    }
    public void IncreaseLevel()
    {
        gameData.Level++;
    }

    public void ChangePosition(Vector3 newPos)
    {
        gameData.Position = newPos;
    }
}
