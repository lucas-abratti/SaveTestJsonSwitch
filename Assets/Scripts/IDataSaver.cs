using UnityEditor;

public interface IDataSaver
{
    public string fileName {  get; }
    public void Save();
    public void Load();
}
