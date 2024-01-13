using UnityEngine;

public class GameSaveData : IDataSaver
{
    public string fileName
    {
        get { return "GameSaveData"; }
    }

    public int level = 12;
    public string name = "User123";
    public Vector3 position = Vector3.down;
    public GameSaveData(int lvl, string n)
    {
        level = lvl;
        name = n;
    }
    public void IncreaseLevel()
    {
        level++;
    }
    public void Save()
    {
        Debug.LogWarning("Starting save from GameSaveData.cs");
        SaveDataManager.Save(fileName, this);
    }
    public void Load()
    {
        Debug.LogWarning("Loading GameSaveData");
        GameSaveData gs = (GameSaveData) SaveDataManager.Load(fileName, typeof(GameSaveData));
        if(gs == null) { return; }
        this.level = gs.level;
        this.name = gs.name;
        this.position = gs.position;
        Debug.LogWarning($"Level: {level}\nName: {name}\nPos: {position}");
    }
}
