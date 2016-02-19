using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    private float speed;

    private Transform cachedTransform;

    // a tweakable value for the time a turn should take
    public float RotationSeconds = 0.7f;

    // the current interpolation t - will move from 0 to 1
    // as the interpolation proceeds
    private float rotationT;

    // the rotations to interpolate between
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    // true when rotating
    private bool isRotating;
    // Use this for initialization
    void Start () {
        speed = 30.0f;
        cachedTransform = transform;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.up * speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.D))
            transform.Rotate(-Vector3.up * speed * Time.deltaTime);
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

}
