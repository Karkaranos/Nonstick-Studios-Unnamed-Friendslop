using FMODUnity;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class RecordMic : MonoBehaviour
{
    //public variables
    [Header("Microphone:")]
    public int RecordingDeviceIndex = 0;
    [TextArea] public string RecordingDeviceName = null;
    [Header("Latency:")]
    public float Latency = 1f;
    public KeyCode PlayAndPause;
    public KeyCode ReverbOnOffSwitch;

    //FMOD objects
    private FMOD.Sound sound;
    private FMOD.CREATESOUNDEXINFO exinfo;
    private FMOD.Channel channel;
    private FMOD.ChannelGroup channelGroup;

    //How many recording devices are plugged in for use
    private int numOfDriversConnected = 0;
    private int numOfDrivers = 0;

    //Info about the microphone
    private System.Guid MicGUID;
    private int SampleRate = 0;
    private FMOD.SPEAKERMODE FMODSpeakerMode;
    private int NumOfChannels = 0;
    private FMOD.DRIVER_STATE driverState;

    //Other variables
    private bool dspEnabled = false;
    private bool playOrPause = true;
    private bool playOkay = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Check to see if any recording devices/drivers are plugged in and available

        RuntimeManager.CoreSystem.getRecordNumDrivers(out numOfDrivers, out numOfDriversConnected);

        if (numOfDriversConnected == 0)
            Debug.Log("No microphones detected");
        else
            Debug.Log(numOfDriversConnected + " microphones available");

        //Get all of the info about the recording device/driver for recording

        RuntimeManager.CoreSystem.getRecordDriverInfo(RecordingDeviceIndex, out RecordingDeviceName, 50, out MicGUID, out SampleRate, out FMODSpeakerMode, out NumOfChannels, out driverState);




    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
