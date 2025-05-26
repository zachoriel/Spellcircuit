using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    #region Singleton

    public static ServiceLocator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(Instance);
    }

    #endregion

    #region Services

    private Dictionary<System.Type, Service> services = new Dictionary<System.Type, Service>();
    [SerializeField] private bool logging = false;

    public T GetService<T>() where T : Service
    {
        if (services.TryGetValue(typeof(T), out Service existingService))
        {
            if (logging) { Debug.Log($"Retrieved existing service of type {typeof(T)}."); }
            return (T)existingService;
        }

        bool foundService = UnityExtensions.TryGetComponentInChildren<T>(this.gameObject, out T service, true);
        if (foundService) 
        { 
            services.Add(typeof(T), service); 
            if (logging) { Debug.Log($"Retrieved new service of type {typeof(T)} from {this.gameObject.name}"); }
            return service;
        }
        else 
        { 
            Debug.LogError($"Service of type {typeof(T)} not found in children of {this.gameObject.name}");
            return null;
        }
    }

    public void AddService<T>(T service) where T : Service
    {
        if (services.ContainsKey(typeof(T)))
        {
            Debug.LogWarning($"Service of type {typeof(T)} already exists in {this.gameObject.name}");
            return;
        }

        services.Add(typeof(T), service);
        if (logging) { Debug.Log($"Added service of type {typeof(T)} to {this.gameObject.name}"); }
    }

    #endregion
}