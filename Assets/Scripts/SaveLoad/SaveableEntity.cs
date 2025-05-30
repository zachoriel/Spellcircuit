using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class SaveableEntity : MonoBehaviour
{
    public string id = string.Empty;

    [ContextMenu("Generate ID")]
    public void GenerateID() => Guid.NewGuid().ToString();

    private SaveLoadSystem saveLoadSystem;
    private SaveLoadSystem.SaveType saveType;

    private void Start()
    {
        saveLoadSystem = ServiceLocator.Instance.GetService<SaveLoadSystem>();
        saveType = saveLoadSystem.CurrentSaveType;
        saveLoadSystem.OnSaveTypeChanged += OnSaveTypeChanged;
    }

    private void OnSaveTypeChanged(SaveLoadSystem.SaveType _newSaveType)
    {
        saveType = _newSaveType;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the SaveType event.
        if (ServiceLocator.Instance.GetService<SaveLoadSystem>() != null)
        {
            ServiceLocator.Instance.GetService<SaveLoadSystem>().OnSaveTypeChanged -= OnSaveTypeChanged;
        }
    }

    public Dictionary<string, string> CaptureState()
    {
        var state = new Dictionary<string, string>();
        foreach (var saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = JsonConvert.SerializeObject(saveable.CaptureState());
        }

        return state;
    }

    public void RestoreState(Dictionary<string, string> _state)
    {
        var stateDicitonary = _state;
        foreach (var saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();
            if (stateDicitonary.TryGetValue(typeName, out string value))
            {
                saveable.RestoreState(value);
            }
        }
    }
}