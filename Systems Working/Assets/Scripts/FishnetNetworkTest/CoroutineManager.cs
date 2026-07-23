using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : NetworkSingleton<CoroutineManager>
{
    public List<Coroutine> managedCoroutines = new();

    /// <summary>
    /// Starts a coroutine and adds it to the list of coroutines being managed by this object
    /// </summary>
    /// <param name="coroutine"></param>
    public Coroutine RunCoroutine(IEnumerator coroutine)
    {
        Coroutine newCoroutine = StartCoroutine(coroutine);
        managedCoroutines.Add(newCoroutine);
        return newCoroutine;
    }

    /// <summary>
    /// Stops a managed coroutine
    /// </summary>
    /// <param name="coroutine"></param>
    public void StopManagedCoroutine(Coroutine coroutine)
    {
        if(coroutine != null && managedCoroutines.IndexOf(coroutine) != -1)
        {
            Coroutine toStop = managedCoroutines[managedCoroutines.IndexOf(coroutine)];
            StopCoroutine(toStop);
            managedCoroutines.Remove(coroutine);
        }
    }

    /// <summary>
    /// Stops all managed coroutines from running and clears the list containing them
    /// </summary>
    public void StopAllManagedCoroutines()
    {
        foreach(Coroutine c in managedCoroutines)
        {
            StopCoroutine(c);
        }

        managedCoroutines.Clear();
    }

}
