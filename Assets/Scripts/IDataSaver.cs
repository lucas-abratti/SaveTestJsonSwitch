using UnityEditor;

public interface IDataSaver
{
    //public string fileName {  get; } // name of the file that will store the data (MUST BE UNIQUE)
    public void Save();
    public void Load();
}
