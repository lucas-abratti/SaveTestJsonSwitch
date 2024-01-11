using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class SaveInputController : MonoBehaviour
{
    GameSaveData gameSaveData;
    OptionsSaveData optionsSaveData;

    private void Start()
    {
        gameSaveData = new GameSaveData(0, "User123");
        optionsSaveData = new OptionsSaveData(0.5f, 3);
    }

    public void Save(CallbackContext context)
    {
        if (!context.started) { return; }
        Debug.LogError($"Saving");
        gameSaveData.Save();
        optionsSaveData.Save();
    }
    public void Load(CallbackContext context)
    {
        if (!context.started) { return; }
        Debug.LogError($"Loading");
        gameSaveData.Load();
        optionsSaveData.Load();
    }
    public void IncreaseLevel(CallbackContext context)
    {
        if (!context.started) { return; }
        gameSaveData.IncreaseLevel();
        optionsSaveData.IncreaseVolume();
        Debug.LogError($"{gameSaveData.level} | {optionsSaveData.volume}");
    }
}
