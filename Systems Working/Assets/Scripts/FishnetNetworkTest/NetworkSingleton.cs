using FishNet.Object;
using UnityEngine;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
            Instance = (T)this;
        else
            Destroy(this);
    }
}
