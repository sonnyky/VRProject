using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class MoveToHospital : MonoBehaviour {

    void Start()
    {
        print("Starting " + Time.time);
        StartCoroutine(WaitAndLoad(10.0F));
        print("Before WaitAndPrint Finishes " + Time.time);
    }
    IEnumerator WaitAndLoad(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        print("WaitAndPrint " + Time.time);
        SceneManager.LoadScene("Hospital");

    }
}
