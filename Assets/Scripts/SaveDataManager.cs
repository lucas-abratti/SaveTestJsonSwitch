using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_SWITCH
    private nn.account.Uid userId;
    private const string mountName = "CBGS";

    List<string> initializedFiles = new List<string>();
    private static SaveDataManager instance;
    public static SaveDataManager Instance
    {
        get 
        { 
            if (instance != null)
            {
                return instance;
            }
            else
            {
                return null;
            }
        }
        set 
        {
            if (instance == null)
            {
                instance = value;
            }
            else
            {
                Destroy(value);
            }
        }
    }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        nn.account.Account.Initialize();
        nn.account.UserHandle userHandle = new nn.account.UserHandle();

        if (!nn.account.Account.TryOpenPreselectedUser(ref userHandle))
        {
            nn.Nn.Abort("Failed to open preselected user.");
        }
        nn.Result result = nn.account.Account.GetUserId(ref userId, userHandle);
        result.abortUnlessSuccess();
        result = nn.fs.SaveData.Mount(mountName, userId);
        result.abortUnlessSuccess();
    }
    private void Initialize(string fileName)
    {
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
    public object Load(string fileName, System.Type type)
    {
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
    
    public void Save(string fileName, object obj)
    {
        if (!initializedFiles.Contains(fileName)) { Initialize(fileName); }
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
# else
    private static SaveDataManager instance;
    public static SaveDataManager Instance
    {
        get 
        { 
            if (instance != null)
            {
                return instance;
            }
            else
            {
                return null;
            }
        }
        set 
        {
            if (instance == null)
            {
                instance = value;
            }
            else
            {
                Destroy(value);
            }
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    public void Save(string fileName, object obj)
    {
        throw new NotImplementedException();
    }

    public object Load(string fileName, System.Type type)
    {
        return new NotImplementedException();
    }
#endif
}
