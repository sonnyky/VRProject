using UnityEngine;
using System.Collections;


public class CameraMovement : MonoBehaviour {
    //Parameters to enable speech recognition
    private SpeechRecognizerManager _speechManager = null;
    private string _message = "";
    private string[] resultList;

    //Parameter to get the RoomDoor class
    private RoomDoor roomDoorClass;

    /*
    Attach this script to the CardboardMain in the scene to enable moving the CardboardMain object
    which is the user's body, from other scripts
    */
    private Transform cachedTransform;

    // a tweakable value for the time a turn should take
    private float RotationSeconds = 0.2f;//w.r.t all axes

    //parameters to let the player walk
    private float walkingSpeed; //walking speed in all directions, take care that on scenes with different scales the speed needed may vary.
    private Vector3 walkingDirection;

    //For voice recognition
    private bool _is_listening;
  
    // the current interpolation t - will move from 0 to 1
    // as the interpolation proceeds
    private float rotationT;

    // the rotations to interpolate between
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    // true when rotating
    private bool isRotating;
    private bool moveFlag;
    private CardboardHead cardboardHead;

    //Flashlight object
    public GameObject flashlightOnPlayer;
    private bool isFlashlightOn;
    private bool hasCoin;

    //Parameter to store current head transform, so we can stop it.
    private Transform curHeadTransform;

    //Parameter to hold different Audio Clip
    public AudioClip corridorBgm;
    public AudioClip wakeUp;
    private AudioSource audiosource;
    public AudioClip clockAudio;
    public AudioClip weCanUseThis;
    public AudioClip roomDoorLocked;
    public AudioClip lightSwitch;
    public AudioClip roomTable;
    public AudioClip roomBed;
    public AudioClip lookAtRAm;
    private AndroidJavaClass javaClass;
    private AndroidJavaClass speechClass;
    private AndroidJavaObject activity;
    // Use this for initialization
    void Start () {
        audiosource = gameObject.GetComponentInChildren<AudioSource>();
        moveFlag = false;
        cachedTransform = transform;
        walkingDirection = new Vector3(0, 0, 0);
        walkingSpeed = 0.03f;
        flashlightOnPlayer.GetComponent<Light>().intensity = 0;
        _is_listening = false;

        isFlashlightOn = false;
        hasCoin = false;
        cardboardHead = Camera.main.GetComponent<StereoController>().Head;
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        javaClass = new AndroidJavaClass("tinker.unityplugin.NativePlugin");

        roomDoorClass = GameObject.Find("RoomDoorMain").GetComponent<RoomDoor>();
        
         javaClass = new AndroidJavaClass("tinker.unityplugin.NativePlugin");
         speechClass = new AndroidJavaClass("tinker.unityplugin.SpeechRecognizerPlugin");
         javaClass.CallStatic("showToast", "Test Android Native Plugin");
         
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log("Speech recognition is only available on Android platform.");
            return;
        }

        _is_listening = false;
    }

    public void ReceiveMessageFromAndroid(string message)
    {
       
        javaClass.CallStatic("showToast", message);
        if(message == "Android" || message == "android" || message == "No match")
        {
            _is_listening = false;
        }
        
    }
  
    // Update is called once per frame

    void Update () {

        if (Input.GetKey(KeyCode.A) && _is_listening == false) {
            speechClass.CallStatic("StartListening", activity);

            _is_listening = true;
            _speechManager.StartListening(5, "ja");

        }

        if (Input.GetKey(KeyCode.W))
        {
            walkingDirection.x = transform.localPosition.x + (walkingSpeed * cardboardHead.transform.forward.x);
            walkingDirection.y = transform.localPosition.y + 0;
            walkingDirection.z = transform.localPosition.z + (walkingSpeed * cardboardHead.transform.forward.z);
            transform.localPosition = walkingDirection;
        }
        if (Input.GetKey(KeyCode.S))
        {
            walkingDirection.x = transform.localPosition.x - (walkingSpeed * cardboardHead.transform.forward.x);
            walkingDirection.y = transform.localPosition.y - 0;
            walkingDirection.z = transform.localPosition.z - (walkingSpeed * cardboardHead.transform.forward.z);
           // transform.localPosition += walkingSpeed * -cardboardHead.transform.forward;
           transform.localPosition = walkingDirection;
        }

        if (Input.GetKey(KeyCode.D)) {
            //Disable strafing right for now
            //transform.localPosition += walkingSpeed * (Quaternion.Euler(0, 90, 0) * cardboardHead.transform.forward);
        }
       
    }

    public void PerformRotation()
    {
        // add the time that has passed since our last update
        // divided by the total number of seconds
        // rotationT will start at 0 and become 1 once 
        // rotationSeconds seconds have passed
        rotationT += Time.deltaTime / RotationSeconds;

        // spherically interpolate between the two positions
        cachedTransform.rotation = Quaternion.Slerp(initialRotation, targetRotation, rotationT);
    }
    private void InitializeRotation(Vector3 axis)
    {
        Vector3 correctedAxis = Quaternion.Inverse(initialRotation) * axis;
        Quaternion axisRotation = Quaternion.AngleAxis(90, correctedAxis);
        targetRotation = initialRotation * axisRotation;
    }

    void OnCollisionEnter(Collision col)
    {
        print("collision with something");
        walkingDirection.x = transform.localPosition.x - (3.5f * walkingSpeed * cardboardHead.transform.forward.x);
        walkingDirection.y = transform.localPosition.y - 0;
        walkingDirection.z = transform.localPosition.z - (3.5f * walkingSpeed * cardboardHead.transform.forward.z);
        transform.localPosition = walkingDirection;
    }

    public void toggleFlashLightOnOff(bool turnOnFlashlight)
    {
        if (turnOnFlashlight)
        {
            isFlashlightOn = true;
            flashlightOnPlayer.GetComponent<Light>().intensity = 2;
        }
        else
        {
            
            isFlashlightOn = false;
            flashlightOnPlayer.GetComponent<Light>().intensity = 0;
        }
    }

    public void togglehasCoin(bool hasCoinFlag)
    {
        if (hasCoinFlag)
        {
            hasCoin = true;
        }
        else
        {
            hasCoin = false;
        }
    }

    public bool getCoinStatus() { return hasCoin; }
    public void stopPlayerMovement()
    {
        walkingSpeed = 0;
    }
    public void restartPlayerMovement()
    {
        walkingSpeed = 0.03f;
    }

    void getCurrentHeadPositionAndRotation()
    {
        curHeadTransform = GameObject.Find("Head").transform;
    }
    void keepCurrentHeadPositionAndRotation()
    {
        GameObject.Find("Head").transform.localRotation = curHeadTransform.localRotation;
    }

    public void changeAudioSource(int audioCode)
    {
        switch (audioCode) {
            case 0:
                audiosource.clip = wakeUp;
                break;
            case 1:
                audiosource.clip = corridorBgm;
                break;
            case 2:
                audiosource.clip = clockAudio;
                audiosource.Play();
                break;
            case 3:
                audiosource.clip = weCanUseThis;
                audiosource.Play();
                break;
            case 4:
                audiosource.clip = roomDoorLocked;
                audiosource.Play();
                break;
            case 5:
                audiosource.clip = lightSwitch;
                audiosource.Play();
                break;
            case 6:
                audiosource.clip = roomTable;
                audiosource.Play();
                break;
            case 7:
                audiosource.clip = roomBed;
                audiosource.Play();
                break;
                /*
            case 8:
                audiosource.clip = lookAtRAm;
                audiosource.Play();
                break;
                */
            default:
                break;
        }
    }

    #region MONOBEHAVIOUR
    void OnDestroy()
    {
        if (_speechManager != null)
            _speechManager.Release();
    }

    #endregion

    #region SPEECH_CALLBACKS

    void OnSpeechEvent(string e)
    {
        switch (int.Parse(e))
        {
            case SpeechRecognizerManager.EVENT_SPEECH_READY:
                DebugLog("Ready for speech");
                break;
            case SpeechRecognizerManager.EVENT_SPEECH_BEGINNING:
                DebugLog("User started speaking");
                break;
            case SpeechRecognizerManager.EVENT_SPEECH_END:
                DebugLog("User stopped speaking");
                break;
        }
    }

    void OnSpeechResults(string results)
    {
        _is_listening = false;

        // Need to parse
        resultList = results.Split(new string[] { SpeechRecognizerManager.RESULT_SEPARATOR }, System.StringSplitOptions.None);
        javaClass.CallStatic("showToast", resultList[0]);
        roomDoorClass.InputAnswer(resultList[0]);
        
    }

    void OnSpeechError(string error)
    {
        switch (int.Parse(error))
        {
            case SpeechRecognizerManager.ERROR_AUDIO:
                DebugLog("Error during recording the audio.");
                break;
            case SpeechRecognizerManager.ERROR_CLIENT:
                DebugLog("Error on the client side.");
                break;
            case SpeechRecognizerManager.ERROR_INSUFFICIENT_PERMISSIONS:
                DebugLog("Insufficient permissions. Do the RECORD_AUDIO and INTERNET permissions have been added to the manifest?");
                break;
            case SpeechRecognizerManager.ERROR_NETWORK:
                DebugLog("A network error occured. Make sure the device has internet access.");
                break;
            case SpeechRecognizerManager.ERROR_NETWORK_TIMEOUT:
                DebugLog("A network timeout occured. Make sure the device has internet access.");
                break;
            case SpeechRecognizerManager.ERROR_NO_MATCH:
                DebugLog("No recognition result matched.");
                break;
            case SpeechRecognizerManager.ERROR_NOT_INITIALIZED:
                DebugLog("Speech recognizer is not initialized.");
                break;
            case SpeechRecognizerManager.ERROR_RECOGNIZER_BUSY:
                DebugLog("Speech recognizer service is busy.");
                break;
            case SpeechRecognizerManager.ERROR_SERVER:
                DebugLog("Server sends error status.");
                break;
            case SpeechRecognizerManager.ERROR_SPEECH_TIMEOUT:
                DebugLog("No speech input.");
                break;
            default:
                break;
        }

        _is_listening = false;
    }

    #endregion


    #region DEBUG

    private void DebugLog(string message)
    {
        Debug.Log(message);
        _message = message;
    }

    #endregion

}
