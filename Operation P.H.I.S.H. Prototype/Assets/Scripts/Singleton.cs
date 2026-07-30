/*****************************************************************************
// File Name :         Singleton.cs
// Author :            Kyle Grenier, Toby Schamberger
// Creation Date :     09/29/2021
//
// Brief Description : Defines a class with a single Instance.
*****************************************************************************/
using NaughtyAttributes;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [Foldout("Singleton Settings")]
    [Tooltip("If this gameobject is a duplicate, then destroy it if true")]
    [SerializeField] private bool destroyGameObject = true; 

    [Foldout("Singleton Settings")]
    [SerializeField] private bool dontDestroyOnLoad = false; 

    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(destroyGameObject ? this.gameObject : this);
        }
    }
}