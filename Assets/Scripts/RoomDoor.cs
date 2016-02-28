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
public class RoomDoor : MonoBehaviour
{
    private Vector3 startingPosition;

    private SpeechRecognizerManager _speechManager = null;
    private bool _isListening = false;
    private string _message = "";

    //Properties to handle auto select, based on how long the user gazes at the object
    private float countdownToAutoConfirm;
    private float waitTimeUntilAutoConfirm = 2.0f;
    private bool waitingConfirmationFlag;

    //Properties to handle icon display to indicate user is interacting with the object
    private GameObject exclamationMark, exclamationMarkInstance;
    private Vector3 deltaPos, scale;

    // Properties to handle user picking an object
    private CardboardHead cardboardHead;
    private CameraMovement playerBody; //Class to interact with the player, calling methods in CameraMovement.cs

    //Unused right now
    private Quaternion markQuarternion;

    //Door
    private string[] resultLst;
    public GameObject testGameObject;

    void Start()
    {
        // We pass the game object's name that will receive the callback messages.
        
        cardboardHead = Camera.main.GetComponent<StereoController>().Head;
        playerBody = GameObject.Find("CardboardMain").GetComponent<CameraMovement>();
        scale = new Vector3(0.1f, 0.1f, 0.1f);
        markQuarternion = new Quaternion();
        startingPosition = transform.localPosition;
        deltaPos.x = startingPosition.x;
        deltaPos.y = startingPosition.y + 0.12f;
        deltaPos.z = startingPosition.z;

        exclamationMark = Resources.Load("ExclamationMark") as GameObject;

        waitingConfirmationFlag = false;
        countdownToAutoConfirm = waitTimeUntilAutoConfirm;
        SetGazedAt(false);

        _speechManager = new SpeechRecognizerManager(gameObject.name);
        Debug.Log("Start : " + gameObject.name);
        print("Start :------------------------------------------------" + gameObject.name);
        _isListening = false;
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log("Speech recognition is only available on Android platform.");
            return;
        }

        if (!SpeechRecognizerManager.IsAvailable())
        {
            Debug.Log("Speech recognition is not available on this device.");
            return;
        }
       
    }

    void LateUpdate()
    {
        //check every frame if the object is being gazed upon

        if (Input.GetKey(KeyCode.A))
        {
            //gameObject.GetComponent<ExitDoor>().Open();
            _speechManager.StartListening(5, "ja");
        }
        if (waitingConfirmationFlag)
        {

            countdownToAutoConfirm -= Time.deltaTime;
            if (countdownToAutoConfirm < 0)
            {
                print("Ask the user to say password");
                //Voice?
                _isListening = true;
                Debug.Log("Listening" + gameObject.name);
                print("Listening :------------------------------------------------" + gameObject.name);
                //_speechManager.StartListening(5, "ja");
                _speechManager.StartListening(3, "en-US");
                print("After Listening :------------------------------------------------" + gameObject.name);
                SetGazedAt(false);
            }

        }

        Cardboard.SDK.UpdateState();
        if (Cardboard.SDK.BackButtonPressed)
        {
            Application.Quit();
        }
    }

    public void SetGazedAt(bool gazedAt)
    {
        //GetComponent<Renderer>().material.color = gazedAt ? Color.green : Color.red;

        if (gazedAt)
        {
            print("Looking at object");
            exclamationMarkInstance = GameObject.Instantiate(exclamationMark, deltaPos, markQuarternion) as GameObject;
            exclamationMarkInstance.name = "exclamationMarkInstance";
            exclamationMarkInstance.transform.localScale = scale;

            waitingConfirmationFlag = true;
            print(waitingConfirmationFlag);
        }
        if (!gazedAt)
        {
            print("Not Looking at object");
            waitingConfirmationFlag = false;
            Destroy(GameObject.Find("exclamationMarkInstance"));
            countdownToAutoConfirm = waitTimeUntilAutoConfirm;
        }
    }

    public void Reset()
    {
        transform.localPosition = startingPosition;
    }

    public void ToggleVRMode()
    {
        Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;
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
        Debug.Log("Hello-----------------------");
        _isListening = false;

        // Need to parse
        string[] texts = results.Split(new string[] { SpeechRecognizerManager.RESULT_SEPARATOR }, System.StringSplitOptions.None);
        ;

        DebugLog("Speech results:\n   " + string.Join("\n   ", texts));

        if (texts[0] == "Android" || texts[0] == "android")
        {
            gameObject.GetComponent<ExitDoor>().Open();
        }
    }



    void OnSpeechError(string error)
    {
        print(error);
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
                Debug.Log("some error");
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
