using UnityEngine;
using System.Collections;


public class CameraMovement : MonoBehaviour {
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

    // Use this for initialization
    void Start () {
        moveFlag = false;
        cachedTransform = transform;
        walkingDirection = new Vector3(0, 0, 0);
        walkingSpeed = 0.03f;
        flashlightOnPlayer.GetComponent<Light>().intensity = 0;

        isFlashlightOn = false;
        hasCoin = false;
        cardboardHead = Camera.main.GetComponent<StereoController>().Head;
    }

    // Update is called once per frame
       
    void Update () {

        //check collider to see if we bumped into anything


        if (Input.GetKey(KeyCode.A)) {
            //Disable strafing left for now
            //transform.localPosition += walkingSpeed * (Quaternion.Euler(0, -90, 0) * cardboardHead.transform.forward);
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
}
