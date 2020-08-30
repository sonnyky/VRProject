
using UnityEngine;

public class SpeechRecognizerManager: Singleton
{
    GameObject m_Receiver;

    [SerializeField]
    AndroidJavaClass[] m_NativeClasses;

    public const int EVENT_SPEECH_READY = 0;
    public const int EVENT_SPEECH_BEGINNING = 1;
    public const int EVENT_SPEECH_END = 2;
    public const string RESULT_SEPARATOR = " ";

    public const int ERROR_AUDIO = 10;
    public const int ERROR_CLIENT = 11;
    public const int ERROR_INSUFFICIENT_PERMISSIONS = 12;
    public const int ERROR_NETWORK = 13;
    public const int ERROR_NETWORK_TIMEOUT = 14;
    public const int ERROR_NO_MATCH = 15;
    public const int ERROR_NOT_INITIALIZED = 16;
    public const int ERROR_RECOGNIZER_BUSY = 17;
    public const int ERROR_SERVER = 18;
    public const int ERROR_SPEECH_TIMEOUT = 19;


    public void SetReceiver(GameObject receiver)
    {
        m_Receiver = receiver;
    }

    public void StartListening(string lang)
    {

    }

    /// <summary>
    /// Release the android recognizer plugin
    /// </summary>
    public void Release()
    {

    }
   
}
