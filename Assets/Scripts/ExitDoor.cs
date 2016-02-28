using UnityEngine;
using System.Collections;

public class ExitDoor : MonoBehaviour {

    private float degree = 360.0f;
    private static bool isOpenStart = false;
    private static bool isOpenEnd = false;
    private static Vector3 vec = new Vector3(0.0f, 1.0f, 0.0f);
    private static AudioSource audioSource;

    // Use this for initialization
    void Start () {
	    gameObject.transform.Rotate(new Vector3(0.0f, degree, 0.0f)); // x,y,z
        audioSource = gameObject.GetComponent<AudioSource>();
    }

	// Update is called once per frame
	void Update () {
        if (isOpenStart)
        {
            degree -= 150.0f * Time.deltaTime;
            degree = Mathf.Clamp(degree, 210.0f, 360.0f);
            gameObject.transform.rotation = Quaternion.AngleAxis(degree, vec);
            isOpenEnd = true;
        }
    }

    public static void Open()
    {
        if (!isOpenEnd)
        {
            isOpenStart = true;
            audioSource.Play();
        }
    }

}
