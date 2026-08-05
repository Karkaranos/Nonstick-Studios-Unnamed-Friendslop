using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private EventInstance demoBGM;

    public static MusicManager instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance != null)
        {
            Debug.Log("There is more than one MusicManager in the scene");
        }
        instance = this;

        demoBGM = AudioManager.instance.CreateEventInstance(FMODEvents.instance.demoBGM);

        StopAll();
        demoBGM.start();

        /*if (SceneManager.GetActiveScene().name.Equals("JaxsonEnvironmentGraybox"))
        {
            demoBGM.start();
            return;
        }*/
    }

    private void StopAll()
    {
        demoBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnDisable()
    {
        demoBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}
