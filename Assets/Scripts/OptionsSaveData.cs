using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsSaveData : IDataSaver
{
    private static OptionsSaveData instance;
    public static OptionsSaveData Instance
    {
        get
        {
            instance ??= new OptionsSaveData(OptionsData.DefaultOptions());
            return instance;
        }
    }

    private readonly OptionsData optionsData;

    public OptionsData OptionsData => optionsData;

    public OptionsSaveData(OptionsData data)
    {
        optionsData = data;
    }
   
    public void Load()
    {
        Debug.LogWarning("Loading OptionsSaveData");
        OptionsData loadedData = (OptionsData)SaveDataManager.Load(OptionsData.FileName, typeof(OptionsData));
        if (loadedData == null) 
        {
            Debug.LogError("Failed to load Options Data");
            return; 
        }

        optionsData.Volume = loadedData.Volume;
        optionsData.Language = loadedData.Language;

        Debug.LogWarning($"Loaded Options Data");
        Debug.LogWarning($"Volume: {optionsData.Volume}");
        Debug.LogWarning($"Lang: {optionsData.Language}");
        Debug.LogWarning($"---------------------------------");
    }

    public void Save()
    {
        Debug.LogWarning("Starting save from OptionsSaveData.cs");
        SaveDataManager.Save(OptionsData.FileName, optionsData);
    }
    public void IncreaseVolume()
    {
        optionsData.Volume += 0.5f;
    }

    public void ChangeLanguage(int newLang)
    {
        optionsData.Language = newLang;
    }
}
