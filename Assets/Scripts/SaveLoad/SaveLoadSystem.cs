using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadSystem : Service
{
    private const string KEY = "";
    private const string IV = "";

    public enum SaveType
    {
        PlainTextJSON,
        Encrypted // NYI
    }
    public delegate void SaveTypeChanged(SaveType newSaveType);
    public event SaveTypeChanged OnSaveTypeChanged;
    [SerializeField] private SaveType saveType;
    public SaveType CurrentSaveType
    {
        get => saveType;
        set
        {
            if (saveType != value)
            {
                saveType = value;
                OnSaveTypeChangedInternal(saveType);
                OnSaveTypeChanged?.Invoke(saveType);
            }
        }
    }

    private void OnSaveTypeChangedInternal(SaveType newSaveType)
    {
        Debug.LogWarning("WARNING: ENCRYPTED SAVE DATA IS A WORK IN PROGRESS; KEY/IV ARE UNSAFELY STORED. USE JSON FOR NOW."); // ToDo: Need to figure out secure storage. Having the Key/IV just sitting here in the script is very bad.
        Debug.LogWarning($"Warning: Save type changed to {newSaveType.ToString()}. Please re-save your game now to ensure data integrity.");
    }

    private string BasePath => $"{Application.persistentDataPath}/";

    private void Awake()
    {
        ServiceLocator.Instance.AddService<SaveLoadSystem>(this);
    }

    public void Save(string _fileName, string _extension)
    {
        var state = new Dictionary<string, string>();
        CaptureState(state);
        SaveFile(state, _fileName, _extension);
    }

    public void Load(string _fileName, string _extension)
    {
        var state = LoadFile(_fileName, _extension);
        RestoreState(state);
    }

    private void SaveFile(Dictionary<string, string> _state, string _fileName, string _extension)
    {
        string fullPath = BasePath + _fileName + _extension;
        if (saveType == SaveType.Encrypted)
        {
            WriteEncryptedData(JsonConvert.SerializeObject(_state), fullPath);
        }
        else
        {
            string json = JsonConvert.SerializeObject(_state, Formatting.Indented);
            File.WriteAllText(fullPath, json);
        }
    }

    private Dictionary<string, string> LoadFile(string _fileName, string _extension)
    {
        string fullPath = BasePath + _fileName + _extension;
        if (!File.Exists(fullPath))
        {
            return new Dictionary<string, string>();
        }

        string data = saveType == SaveType.Encrypted ? ReadEncryptedData(fullPath) : File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
    }

    private void CaptureState(Dictionary<string, string> _state)
    {
        // ToDo: Think more on this. Maybe saveables can be cached and this method can add to the list if needed.
        foreach (var saveable in FindObjectsByType<SaveableEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            _state[saveable.id] = JsonConvert.SerializeObject(saveable.CaptureState());
        }
    }

    private void RestoreState(Dictionary<string, string> _state)
    {
        // ToDo: See CaptureState comment above.
        foreach (var saveable in FindObjectsByType<SaveableEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (_state.TryGetValue(saveable.id, out string value))
            {
                saveable.RestoreState(JsonConvert.DeserializeObject<Dictionary<string, string>>(value));
            }
        }
    }

    public void WriteEncryptedData(string _state, string _fullPath)
    {
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        using ICryptoTransform encryptor = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new CryptoStream(
            File.OpenWrite(_fullPath),
            encryptor,
            CryptoStreamMode.Write
        );

        cryptoStream.Write(Encoding.UTF8.GetBytes(_state));
    }

    public string ReadEncryptedData(string _fullPath)
    {
        byte[] fileBytes = File.ReadAllBytes(_fullPath);
        using Aes aesProvider = Aes.Create();

        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform decryptor = aesProvider.CreateDecryptor(
            aesProvider.Key,
            aesProvider.IV
        );
        using MemoryStream decryptionStream = new MemoryStream(fileBytes);
        using CryptoStream cryptoStream = new CryptoStream(
            decryptionStream,
            decryptor,
            CryptoStreamMode.Read
        );
        using StreamReader reader = new StreamReader(cryptoStream);

        string result = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<string>(result);
    }
}