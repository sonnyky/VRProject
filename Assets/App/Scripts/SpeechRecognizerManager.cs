
using UnityEngine;

public class SpeechRecognizerManager: Singleton
{
    GameObject m_Receiver;

    [SerializeField]
    AndroidJavaClass[] m_NativeClasses;

    public const int EVENT_SPEECH_READY = 0;
    public const int EVENT_SPEECH_BEGINNING = 1;
    public const int EVENT_SPEECH_END = 2;

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
