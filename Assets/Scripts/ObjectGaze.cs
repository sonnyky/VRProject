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

    private float autoConfirmationTime;
    private bool waitingConfirmationFlag;

    private GameObject exclamationMark, exclamationMarkInstance;
    private Vector3 deltaPos;
    private Quaternion markQuarternion;
    void Start()
    {
        markQuarternion = new Quaternion();
        startingPosition = transform.localPosition;
        deltaPos.x = startingPosition.x;
        deltaPos.y = startingPosition.y + 0.12f;
        deltaPos.z = startingPosition.z;
        exclamationMark = (GameObject)Resources.Load("Prefab/ExclamationMark");
        exclamationMark.transform.localPosition = deltaPos;
        waitingConfirmationFlag = false;
        autoConfirmationTime = 2.0f;
        SetGazedAt(false);
    }

    void LateUpdate()
    {
        if (waitingConfirmationFlag)
        {
            autoConfirmationTime -= Time.deltaTime;
            if (autoConfirmationTime < 0)
            {
                print("Selected by gazing more than 2 seconds");
                //Selected, go to the Hospital scene
                SceneManager.LoadScene("Hospital");
                waitingConfirmationFlag = false;
                autoConfirmationTime = 2.0f;
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
            autoConfirmationTime = 2.0f;
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
