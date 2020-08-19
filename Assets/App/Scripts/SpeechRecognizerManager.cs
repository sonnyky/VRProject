
using UnityEngine;

public class SpeechRecognizerManager: Singleton
{
    GameObject m_Receiver;

    [SerializeField]
    AndroidJavaClass[] m_NativeClasses; 

    public void SetReceiver(GameObject receiver)
    {
        m_Receiver = receiver;
    }
   
}
