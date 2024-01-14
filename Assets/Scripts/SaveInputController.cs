using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class SaveInputController : MonoBehaviour
{
    GameSaveData gameSaveData = GameSaveData.Instance;
    OptionsSaveData optionsSaveData = OptionsSaveData.Instance;

    public void Save(CallbackContext context)
    {
        if (!context.started) { return; }

        Debug.LogWarning($"Saving...");
        gameSaveData.Save();
        optionsSaveData.Save();
    }
    public void Load(CallbackContext context)
    {
        if (!context.started) { return; }
        Debug.LogWarning($"Loading...");
        gameSaveData.Load();
        optionsSaveData.Load();         
    }
    public void ChangeValues(CallbackContext context)
    {
        if (!context.started) { return; }

        gameSaveData.IncreaseLevel();
        gameSaveData.ChangePosition(RandVector3());
        optionsSaveData.IncreaseVolume();
        optionsSaveData.ChangeLanguage(Random.Range(1, 10));

        Debug.LogWarning("---------------------------------------------");
        Debug.LogWarning($"Level: {gameSaveData.GameData.Level}");
        Debug.LogWarning($"Position: {gameSaveData.GameData.Position}");
        Debug.LogWarning($"Volume: {optionsSaveData.OptionsData.Volume}");
        Debug.LogWarning($"Language: {optionsSaveData.OptionsData.Language}");
        Debug.LogWarning("---------------------------------------------");
    }

    private Vector3 RandVector3()
    {
        float randomX = Random.Range(-100f, 100f);
        float randomY = Random.Range(-100f, 100f);
        float randomZ = Random.Range(-100f, 100f);

        return new(randomX, randomY, randomZ);
    }

}
