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

    //Properties to handle auto select, based on how long the user gazes at the object
    private float countdownToAutoConfirm;
    private float waitTimeUntilAutoConfirm = 4.0f;
    private bool waitingConfirmationFlag;

    //Properties to handle icon display to indicate user is interacting with the object
    private GameObject exclamationMark, exclamationMarkInstance;
    private Vector3 deltaPos;

    // Properties to handle user moving in closer to the object
    private float countdownToAutoMove;
    private float waitTimeUntilAutoMove = 2.0f;
    private bool moveCloserToObject;
    private CardboardHead cardboardHead;
    private CameraMovement playerBody; //weird naming, but camera is effectively player's body

    //Unused right now
    private Quaternion markQuarternion;

    void Start()
    {
        moveCloserToObject = false;
        cardboardHead = Camera.main.GetComponent<StereoController>().Head;
        playerBody =  GameObject.Find("CardboardMain").GetComponent<CameraMovement>();

        markQuarternion = new Quaternion();
        startingPosition = transform.localPosition;
        deltaPos.x = startingPosition.x;
        deltaPos.y = startingPosition.y + 0.12f;
        deltaPos.z = startingPosition.z;
        exclamationMark = (GameObject)Resources.Load("Prefab/ExclamationMark");
        exclamationMark.transform.localPosition = deltaPos;
        waitingConfirmationFlag = false;
        countdownToAutoConfirm = waitTimeUntilAutoConfirm;
        countdownToAutoMove = waitTimeUntilAutoMove;
        SetGazedAt(false);
    }

    void LateUpdate()
    {
        //check every frame if the object is being gazed upon
        RaycastHit hit;
        bool isLookedAt = GetComponent<Collider>().Raycast(cardboardHead.Gaze, out hit, Mathf.Infinity);

        if (waitingConfirmationFlag && moveCloserToObject == false)
        {
            if (isLookedAt)
            {
                countdownToAutoMove -= Time.deltaTime;
                if (countdownToAutoMove < 0)
                {
                    print("gazing more than 2 seconds");
                    //If we are not close to the object, then we move closer to the object
                    if (moveCloserToObject == false)
                    {
                        print("move in closer to object");
                        //Call public method of the player body to move it closer to this object
                        moveCloserToObject = true;
                        print(hit.point);
                        playerBody.zoomIn(true);
                    }
                }
            }
        }else if (waitingConfirmationFlag && moveCloserToObject == true)
        {
            //We are close to the object and still gazing at it
            countdownToAutoConfirm -= Time.deltaTime;
            if(countdownToAutoConfirm < 0)
            {
                //Selected, do something
                SceneManager.LoadScene("Hospital");
                countdownToAutoConfirm = waitTimeUntilAutoConfirm;
            }

            //Check distance
            if(hit.distance < 0.04)
            {
                playerBody.zoomIn(false);
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
            exclamationMarkInstance = GameObject.Instantiate(exclamationMark, exclamationMark.transform.localPosition, markQuarternion) as GameObject;
            exclamationMarkInstance.name = "exclamationMarkInstance";

            waitingConfirmationFlag = true;
        }
        else
        {
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
}
