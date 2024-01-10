using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsSaveData : IDataSaver
{
    public float volume = 1.0f;
    public int language = 3;
    public OptionsSaveData(float vol, int lang)
    {
        this.volume = vol;
        this.language = lang;
    }
    public string fileName
    {
        get { return "OptionsSaveData"; }
    }
    public void Load()
    {
        OptionsSaveData os = (OptionsSaveData)SaveDataManager.Instance.Load(fileName, typeof(OptionsSaveData));
        if (os == null) { return; }
        Debug.LogError($"{os.volume}\n{os.language}");
    }

    public void Save()
    {
        SaveDataManager.Instance.Save(fileName, this);
    }
    public void IncreaseVolume()
    {
        volume += 0.5f;
    }
}
