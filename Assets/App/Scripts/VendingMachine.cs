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
public class VendingMachine : MonoBehaviour
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
    private CameraMovement playerBody; //Class to interact with the player, calling methods in CameraMovement.cs

    //Unused right now
    private Quaternion markQuarternion;

    void Start()
    {
        // We pass the game object's name that will receive the callback messages.
        _speechManager = new SpeechRecognizerManager();
        _speechManager.SetReceiver(gameObject);
        _isListening = false;
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

        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log("Speech recognition is only available on Android platform.");
            return;
        }
    }

    void LateUpdate()
    {
        //check every frame if the object is being gazed upon


        if (waitingConfirmationFlag)
        {

            countdownToAutoConfirm -= Time.deltaTime;
            if (countdownToAutoConfirm < 0)
            {
                print("use coin if player has it");
                if (playerBody.getCoinStatus()) {
                    //Player has a coin !

                }
                else
                {
                    //Player does not have a coin

                }
                SetGazedAt(false);
            }

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

    public void TeleportRandomly()
    {
        Vector3 direction = Random.onUnitSphere;
        direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
        float distance = 2 * Random.value + 1.5f;
        transform.localPosition = direction * distance;
    }
}
