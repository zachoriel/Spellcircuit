using UnityEngine;

public class ImportantObjectHolder : Service
{
    public GameObject player;

    private void Awake()
    {
        ServiceLocator.Instance.AddService<ImportantObjectHolder>(this);
    }
}
