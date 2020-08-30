// Copyright 2014 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


//This script is based on Google's sample so I'm leaving the above licensing.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ObjectGaze : MonoBehaviour
{
    private Vector3 startingPosition;

    private SpeechRecognizerManager _speechManager = null;
    private bool _isListening = false;
    private string _message = "";

    //Properties to handle auto select, based on how long the user gazes at the object
    private float countdownToAutoConfirm;
    private float waitTimeUntilAutoConfirm = 4.0f;
    private bool waitingConfirmationFlag;

    //Properties to handle icon display to indicate user is interacting with the object
    private GameObject exclamationMark, exclamationMarkInstance;
    private Vector3 deltaPos;

    // Properties to handle user moving in closer to the object
    private float countdownToVoiceInput;
    private float waitTimeUntilVoiceInput = 2.0f;
    private bool moveCloserToObject;
    private CameraMovement playerBody; //weird naming, but camera is effectively player's body

    //Unused right now
    private Quaternion markQuarternion;

    void Start()
    {
        // We pass the game object's name that will receive the callback messages.
        _speechManager = new SpeechRecognizerManager();
        _speechManager.SetReceiver(gameObject);
        _isListening = false;
        moveCloserToObject = false;
        playerBody =  GameObject.Find("CardboardMain").GetComponent<CameraMovement>();

        markQuarternion = new Quaternion();
        startingPosition = transform.localPosition;
        deltaPos.x = startingPosition.x;
        deltaPos.y = startingPosition.y + 0.12f;
        deltaPos.z = startingPosition.z;

        exclamationMark = Resources.Load("ExclamationMark") as GameObject;

        waitingConfirmationFlag = false;
        countdownToAutoConfirm = waitTimeUntilAutoConfirm;
        countdownToVoiceInput = waitTimeUntilVoiceInput;
        SetGazedAt(false);

        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log("Speech recognition is only available on Android platform.");
            return;
        }
    }

    void LateUpdate()
    {
      
        if (waitingConfirmationFlag)
        {
           
            countdownToVoiceInput -= Time.deltaTime;
            if (countdownToVoiceInput < 0 && _isListening == false)
            {
                print("gazing more than 2 seconds");
                _isListening = true;
                _speechManager.StartListening("en-US");

            }
            
        }
    }

    public void SetGazedAt(bool gazedAt)
    {
        //GetComponent<Renderer>().material.color = gazedAt ? Color.green : Color.red;

        if (gazedAt)
        {

            exclamationMarkInstance = GameObject.Instantiate(exclamationMark, deltaPos, markQuarternion) as GameObject;
            exclamationMarkInstance.name = "exclamationMarkInstance";
            print("Looking at object");
            waitingConfirmationFlag = true;
        }
        else
        {
          
            waitingConfirmationFlag = false;
            Destroy(GameObject.Find("exclamationMarkInstance"));
            print("Looking at object");
            countdownToAutoConfirm = waitTimeUntilAutoConfirm;
            countdownToVoiceInput = waitTimeUntilVoiceInput;
        }
    }

    public void Reset()
    {
        transform.localPosition = startingPosition;
    }

    public void TeleportRandomly()
    {
        Vector3 direction = Random.onUnitSphere;
        direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
        float distance = 2 * Random.value + 1.5f;
        transform.localPosition = direction * distance;
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
        _isListening = false;

        // Need to parse
        string[] texts = results.Split(new string[] { SpeechRecognizerManager.RESULT_SEPARATOR }, System.StringSplitOptions.None);
        ;

        DebugLog("Speech results:\n   " + string.Join("\n   ", texts));

        if(texts[0] == "Android" || texts[0] == "android")
        {
            SceneManager.LoadScene("Hospital");
        }
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

        _isListening = false;
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
