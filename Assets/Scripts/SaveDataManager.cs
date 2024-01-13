using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveDataManager
{
#if !UNITY_EDITOR && UNITY_SWITCH // Switch implementation
    private const string mountName = "CBGS"; // Unique name for each game

    static List<string> initializedFiles = new List<string>(); //Stores Files that have already been initialized since the app launched

    private static bool isMounted = false;

    private static nn.account.Uid userId;

    private static void MountSaveData()
    {
        isMounted = true;
        nn.account.Account.Initialize();
        nn.account.UserHandle userHandle = new nn.account.UserHandle();

        Debug.LogError("Trying to open preselected User");

        if (!nn.account.Account.TryOpenPreselectedUser(ref userHandle))
        {
            nn.Nn.Abort("Failed to open preselected user.");
        }
        nn.Result result = nn.account.Account.GetUserId(ref userId, userHandle);
        result.abortUnlessSuccess();
        result = nn.fs.SaveData.Mount(mountName, userId);
        result.abortUnlessSuccess();
    }
    private static void InitializeFile(string fileName)
    {
        if (!isMounted) { MountSaveData(); }
        initializedFiles.Add(fileName);
        string filePath = mountName + ":/" + fileName;
        nn.fs.EntryType entryType = 0;
        nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);
        if (result.IsSuccess())
        {
            return;
        }
        if (!nn.fs.FileSystem.ResultPathNotFound.Includes(result))
        {
            result.abortUnlessSuccess();
        }

        byte[] data;
        using (BinaryWriter writer = new BinaryWriter(new MemoryStream(sizeof(int))))
        {
            writer.Write(0);
            writer.BaseStream.Close();
            data = (writer.BaseStream as MemoryStream).GetBuffer();
            Debug.Assert(data.Length == sizeof(int));
        }

#if UNITY_SWITCH
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif
        nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();

        result = nn.fs.File.Create(filePath, sizeof(int));
        result.abortUnlessSuccess();

        result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Write);
        result.abortUnlessSuccess();

        const int offset = 0;
        result = nn.fs.File.Write(fileHandle, offset, data, data.LongLength, nn.fs.WriteOption.Flush);
        result.abortUnlessSuccess();

        nn.fs.File.Close(fileHandle);

        result = nn.fs.FileSystem.Commit(mountName);
        result.abortUnlessSuccess();

#if UNITY_SWITCH
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
    }
    public static object Load(string fileName, System.Type type)
    {
        if (!isMounted) { MountSaveData(); }
        string filePath = mountName + ":/" + fileName;
        nn.fs.EntryType entryType = 0;
        nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);
        if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
        {
            return null;
        }
        result.abortUnlessSuccess();
        
        nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();

        result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
        result.abortUnlessSuccess();

        long fileSize = 0;
        result = nn.fs.File.GetSize(ref fileSize, fileHandle);
        result.abortUnlessSuccess();

        byte[] data = new byte[fileSize];
        result = nn.fs.File.Read(fileHandle, 0, data, fileSize);
        result.abortUnlessSuccess();

        nn.fs.File.Close(fileHandle);

        using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
        {
            string dataString = reader.ReadString();
            return JsonUtility.FromJson(dataString, type);
        }
    }
    
    public static void Save(string fileName, object obj)
    {
        if (!initializedFiles.Contains(fileName)) { InitializeFile(fileName); }
        string filePath = mountName + ":/" + fileName;
        string dataToSave = JsonUtility.ToJson(obj);
        byte[] data;
        using (BinaryWriter writer = new BinaryWriter(new MemoryStream(sizeof(char) * dataToSave.Length)))
        {
            writer.Write(dataToSave);
            writer.BaseStream.Close();
            data = (writer.BaseStream as MemoryStream).GetBuffer();
        }

#if UNITY_SWITCH
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif
        nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();

        nn.Result result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.AllowAppend | nn.fs.OpenFileMode.Write);
        result.abortUnlessSuccess();

        const int offset = 0;
        result = nn.fs.File.Write(fileHandle, offset, data, data.LongLength, nn.fs.WriteOption.Flush);
        result.abortUnlessSuccess();

        nn.fs.File.Close(fileHandle);
        result = nn.fs.FileSystem.Commit(mountName);
        result.abortUnlessSuccess();

#if UNITY_SWITCH
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
    }
# else //PC implementation

    // Carpetas y subcarpetas de guardado para mantener organizacion
    private const string Save_Folder = "/Saves/";
    private const string Default_Sub_Folder = "Default";

    // Retorna la ruta completa de guardado
    public static string GetSavePath(string subFolder = Default_Sub_Folder)
    {
        // Persistent Data Path
        // Win --> %userprofile%\AppData\LocalLow\<companyname>\<productname>
        string savePath = Application.persistentDataPath + Save_Folder;
        savePath += subFolder + "/";
        return savePath;
    }

    // Verifica si el archivo de guardado ya existe
    public static bool IsSavedFileExistent(string fileName, string subFolder = Default_Sub_Folder)
    {
        string savePath = GetSavePath(subFolder);
        return File.Exists(savePath + fileName);
    }

    // Guarda un archivo a disco utilizando el guardado elegido, cierra FileStream al final
    public static void Save(string fileName, object objectToSave)
    {
        string savePath = GetSavePath("JSON");

        // Si hace falta, crea una carpeta para guardado.
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        FileStream saveFile = File.Create(savePath + fileName + ".json");
        
        // Serializa el objeto con los datos a un JSON
        string json = JsonUtility.ToJson(objectToSave);

        // Escribe y automaticamente cierra FileStream y StreamWriter
        using StreamWriter streamWriter = new(saveFile, System.Text.Encoding.UTF8);
        streamWriter.Write(json);
    }

    public static object Load(string fileName, System.Type type)
    {
        string savePath = GetSavePath("JSON");

        // Si no existe el archivo ha ocurrido algun error
        if (!File.Exists(savePath + fileName + ".json"))
        {
            Debug.LogError($"SaveManager: No existe el archivo {fileName} en: {savePath}");
            return null;
        }

        FileStream saveFile = File.OpenRead(savePath + fileName + ".json");
        using StreamReader streamReader = new(saveFile, System.Text.Encoding.UTF8);
        string json = streamReader.ReadToEnd();
        object data = JsonUtility.FromJson(json, type);

        // Si esta todo OK retorna la data a cargar
        return data;
    }
#endif
}
