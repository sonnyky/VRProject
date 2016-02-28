﻿// Copyright 2014 Google Inc. All rights reserved.
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
public class SwitchBox : MonoBehaviour
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

    //Parameters to control the lights
    public GameObject pointlightbedroom;
    public GameObject spotlightbedroom;
    public GameObject pointlightbedroomposter;
    void Start()
    {
        // We pass the game object's name that will receive the callback messages.
        _speechManager = new SpeechRecognizerManager(gameObject.name);
        _isListening = false;
        toggleBedroomLights(false);
        cardboardHead = Camera.main.GetComponent<StereoController>().Head;
        playerBody = GameObject.Find("CardboardMain").GetComponent<CameraMovement>();

        pointlightbedroom.GetComponent<Light>().intensity = 0;
        spotlightbedroom.GetComponent<Light>().intensity = 0;
        pointlightbedroomposter.GetComponent<Light>().intensity = 0;

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

        if (!SpeechRecognizerManager.IsAvailable())
        {
            Debug.Log("Speech recognition is not available on this device.");
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
                //print("gazing more than 2 seconds");

                //This part for bedroom lights
                toggleBedroomLights(true);
                playerBody.changeAudioSource(5);
                SetGazedAt(false);
               // Destroy(gameObject);
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
            //print(waitingConfirmationFlag);
        }
        if (!gazedAt)
        {
            //print("Not Looking at object");
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

    void toggleBedroomLights(bool switchLights)
    {
        if (switchLights)
        {
            pointlightbedroom.GetComponent<Light>().intensity = 0.5f;
            spotlightbedroom.GetComponent<Light>().intensity = 0.5f;
            pointlightbedroomposter.GetComponent<Light>().intensity = 0.5f;
        }
        else
        {
            pointlightbedroom.GetComponent<Light>().intensity = 0;
            spotlightbedroom.GetComponent<Light>().intensity = 0;
            pointlightbedroomposter.GetComponent<Light>().intensity = 0;

        }
    }
}
